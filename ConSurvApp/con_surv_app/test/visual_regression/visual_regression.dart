/// Support for the visual-regression-tests of ConSurvApp.
///
/// This codeunit does not render in a browser, so there is no "engine"-dimension a baseline has to be split by: the
/// widget-tree is rasterized by Flutter's own Skia-based renderer, which is expected to produce the same pixels
/// regardless of the host it runs on. Reproducibility across Windows and Linux is nevertheless not guaranteed on an
/// arbitrary developer-machine (the version of the Flutter-SDK and therefore of the fonts it ships differ), so both
/// the generation and the assertion of the baselines are only expected to be reproducible when they run inside the
/// same container (see the "SCBuilder"-image used by
/// TFCPS_CodeUnitSpecific_Flutter_Functions.run_visual_regression_tests_in_container); running them directly on a
/// host is useful during development but not authoritative.
///
/// Every test which uses [expectAppToLookLikeBaseline] must pass `tags: visualRegressionTestTag` to `testWidgets`, so
/// that the normal `flutter test` (which the non-visual part of `RunTestcases.py` runs) can exclude them: comparing
/// against a baseline which was generated inside the container would otherwise be flaky on a developer's own machine.
/// The container-based scripts run them explicitly with `--tags visual-regression --run-skipped` instead (see
/// `dart_test.yaml`).
library;

import 'dart:io';
import 'dart:typed_data';
import 'dart:ui' as ui;

import 'package:con_surv_app/app_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:image/image.dart' as img;
import 'package:path/path.dart' as path;

/// The tag every visual-regression-testcase has to be declared with. See the library-comment above.
const String visualRegressionTestTag = 'visual-regression';

/// Amount of pixels (after the downscaling described at [_downscaleFactor]) a screenshot is allowed to differ from
/// its baseline. Chosen as a compromise: small enough to catch a real change of layout or color (which affects whole
/// regions, i.e. hundreds of downscaled pixels), large enough to tolerate the antialiasing- and
/// JPEG-compression-noise which remains even between two renders of the exact same widget-tree.
const int maximalAmountOfDifferentPixels = 40;

/// The factor screenshots are downscaled by (using a box/average-filter) before they are compared. Averaging a block
/// of pixels spreads out small per-pixel rendering-noise so it stays below [_maximalDifferencePerColorChannel], while
/// a part of the page which really changed keeps differing.
const int _downscaleFactor = 8;

/// The per-channel (red/green/blue) difference (0-255) a pixel has to exceed to count as "different" at all.
const int _maximalDifferencePerColorChannel = 32;

/// The JPEG-quality (0-100) the baselines are encoded with when they are (re-)generated.
const int _baselineJpegQuality = 92;

/// Wraps [child] in a [MaterialApp] which provides the [Theme] (and [Navigator], [Directionality], ...) a page needs,
/// using both themes of the app ([buildThemeOfTheApp]). Which one applies follows the platform-brightness
/// ([ThemeMode.system]), exactly as the real app does, so a visual-regression-test can cover [child] in both the
/// light- and the dark-theme by switching the platform-brightness (see [expectAppToLookLikeBaseline]).
Widget wrapWithApp(Widget child) {
  return MaterialApp(
    theme: buildThemeOfTheApp(Brightness.light),
    darkTheme: buildThemeOfTheApp(Brightness.dark),
    themeMode: ThemeMode.system,
    home: child,
  );
}

Directory _baselinesDirectory() {
  // `flutter test` always runs with the current directory set to the package which contains the pubspec.yaml, i.e.
  // "ConSurvApp/con_surv_app". The baselines live next to it, at the codeunit-level, so that they are not specific to
  // this one flutter-package.
  final String codeunitFolder = Directory.current.parent.path;
  return Directory(path.join(codeunitFolder, 'Other', 'Resources', 'VisualRegressionBaselines'));
}

Directory _failuresDirectory() {
  final String codeunitFolder = Directory.current.parent.path;
  return Directory(path.join(codeunitFolder, 'Other', 'Artifacts', 'VisualRegressionTestResults'));
}

/// Pumps and settles [app], then asserts that it looks like its baseline-images [baselineName].
///
/// Every scenario is covered in both the light- and the dark-theme: [app] is rendered and asserted once with the
/// platform-brightness set to light and once set to dark, so that an unintended visual change is caught in whichever
/// theme it occurs in. The two runs write two baseline-images whose filenames begin with the theme-mode they were
/// rendered in, as the prefix `light-` or `dark-` (so [baselineName] `settings-page` becomes `light-settings-page`
/// and `dark-settings-page`). [app] has to follow the platform-brightness ([ThemeMode.system], see [wrapWithApp]) so
/// the same [app] renders in both themes without the caller having to build it a second time.
///
/// If `flutter test --update-goldens` was used to run the test (which sets [autoUpdateGoldenFiles]) the baselines are
/// (re-)generated from the current render instead of being compared against.
Future<void> expectAppToLookLikeBaseline(
  WidgetTester tester,
  Widget app,
  String baselineName, {
  Size surfaceSize = const Size(1000, 1200),
}) async {
  await _expectAppToLookLikeBaselineInTheme(tester, app, baselineName, Brightness.light, surfaceSize);
  await _expectAppToLookLikeBaselineInTheme(tester, app, baselineName, Brightness.dark, surfaceSize);
}

Future<void> _expectAppToLookLikeBaselineInTheme(
  WidgetTester tester,
  Widget app,
  String baselineName,
  Brightness brightness,
  Size surfaceSize,
) async {
  final String themedBaselineName = '${brightness == Brightness.light ? 'light' : 'dark'}-$baselineName';

  // The theme [wrapWithApp] shows follows the platform-brightness ([ThemeMode.system]), so setting it here is what
  // selects the light- or the dark-theme for this render.
  tester.platformDispatcher.platformBrightnessTestValue = brightness;
  addTearDown(tester.platformDispatcher.clearPlatformBrightnessTestValue);
  tester.view.physicalSize = surfaceSize;
  tester.view.devicePixelRatio = 1.0;
  addTearDown(tester.view.reset);

  await tester.pumpWidget(app);
  await tester.pumpAndSettle();

  // Both calls have to run inside the same `runAsync` (mirroring `matchesGoldenFile`'s own implementation):
  // `toImage`/`toByteData` complete via a real platform-callback, which never fires while only the fake-async zone of
  // `testWidgets` is pumped. Calling them unwrapped does not throw, it returns a premature, truncated image and then
  // leaves that callback pending forever, which makes the whole test hang on exit.
  final ByteData? byteData = await tester.runAsync<ByteData?>(() async {
    final ui.Image capturedImage = await captureImage(tester.element(find.byWidget(app)));
    final ByteData? data = await capturedImage.toByteData(format: ui.ImageByteFormat.png);
    capturedImage.dispose();
    return data;
  });
  final img.Image? rendered = img.decodePng(byteData!.buffer.asUint8List());
  if (rendered == null) {
    fail('The screenshot of "$themedBaselineName" could not be decoded as a PNG-image, which should be impossible.');
  }

  final File baselineFile = File(path.join(_baselinesDirectory().path, '$themedBaselineName.jpg'));

  if (autoUpdateGoldenFiles) {
    baselineFile.parent.createSync(recursive: true);
    baselineFile.writeAsBytesSync(img.encodeJpg(rendered, quality: _baselineJpegQuality));
    await _resetTreeForTheNextTheme(tester);
    return;
  }

  if (!baselineFile.existsSync()) {
    fail(
      'No baseline-image exists for "$themedBaselineName" (expected at "${baselineFile.path}"). Generate it with '
      '"task UpdateVisualRegressionBaselines" and commit it.',
    );
  }
  final img.Image? baseline = img.decodeJpg(baselineFile.readAsBytesSync());
  if (baseline == null) {
    fail('The baseline-image "${baselineFile.path}" could not be decoded as a JPEG-image.');
  }

  if (rendered.width != baseline.width || rendered.height != baseline.height) {
    fail(
      'The screenshot of "$themedBaselineName" (${rendered.width}x${rendered.height}) has a different size than its '
      'baseline (${baseline.width}x${baseline.height}).',
    );
  }

  final int differentPixels = _countDifferentPixels(rendered, baseline);
  if (maximalAmountOfDifferentPixels < differentPixels) {
    final File actualFile = _preserveFailureArtifact(themedBaselineName, 'actual', rendered);
    final File diffFile = _preserveFailureArtifact(themedBaselineName, 'diff', _visualizeDifference(rendered, baseline));
    fail(
      'The screenshot of "$themedBaselineName" differs from its baseline in $differentPixels pixel(s) after '
      'downscaling (allowed: $maximalAmountOfDifferentPixels). The actual render was written to "${actualFile.path}" '
      'and a visualization of the difference to "${diffFile.path}"; compare them with the baseline at '
      '"${baselineFile.path}". If the change of appearance was intended, regenerate the baseline with '
      '"task UpdateVisualRegressionBaselines".',
    );
  }
  await _resetTreeForTheNextTheme(tester);
}

/// Tears the rendered widget-tree down between the two theme-runs of [expectAppToLookLikeBaseline], so that the second
/// run mounts [app] from scratch (a fresh set of [State]-objects) instead of updating the first run's tree in place.
Future<void> _resetTreeForTheNextTheme(WidgetTester tester) async {
  await tester.pumpWidget(const SizedBox.shrink());
  await tester.pumpAndSettle();
}

int _countDifferentPixels(img.Image rendered, img.Image baseline) {
  final img.Image downscaledRendered = _downscale(rendered);
  final img.Image downscaledBaseline = _downscale(baseline);
  int differentPixels = 0;
  for (int y = 0; y < downscaledRendered.height; y++) {
    for (int x = 0; x < downscaledRendered.width; x++) {
      if (_maximalDifferencePerColorChannel < _channelDifference(downscaledRendered.getPixel(x, y), downscaledBaseline.getPixel(x, y))) {
        differentPixels++;
      }
    }
  }
  return differentPixels;
}

img.Image _downscale(img.Image image) {
  if (_downscaleFactor < 2) {
    return image;
  }
  return img.copyResize(
    image,
    width: (image.width / _downscaleFactor).ceil().clamp(1, image.width),
    height: (image.height / _downscaleFactor).ceil().clamp(1, image.height),
    interpolation: img.Interpolation.average,
  );
}

/// The biggest difference of the red-, green- and blue-channel of the two pixels. The alpha-channel is ignored: the
/// captured screenshot is fully opaque, and comparing it against a decoded JPEG (which never has an alpha-channel)
/// would otherwise always report a difference.
int _channelDifference(img.Pixel a, img.Pixel b) {
  final int redDifference = (a.r - b.r).abs().round();
  final int greenDifference = (a.g - b.g).abs().round();
  final int blueDifference = (a.b - b.b).abs().round();
  return <int>[redDifference, greenDifference, blueDifference].reduce((int value, int element) => value > element ? value : element);
}

/// Produces an image the same size as [rendered], where every pixel whose (upscaled) block differed from the baseline
/// by more than [_maximalDifferencePerColorChannel] is painted red and everything else is a dimmed, grayscale copy of
/// [rendered] - so a reviewer immediately sees which area of the page changed.
img.Image _visualizeDifference(img.Image rendered, img.Image baseline) {
  final img.Image downscaledRendered = _downscale(rendered);
  final img.Image downscaledBaseline = _downscale(baseline);
  final img.Image visualization = img.Image.from(rendered)..convert(numChannels: 3);
  final double blockWidth = rendered.width / downscaledRendered.width;
  final double blockHeight = rendered.height / downscaledRendered.height;
  for (int y = 0; y < rendered.height; y++) {
    for (int x = 0; x < rendered.width; x++) {
      final int blockX = (x / blockWidth).floor().clamp(0, downscaledRendered.width - 1);
      final int blockY = (y / blockHeight).floor().clamp(0, downscaledRendered.height - 1);
      final bool differs = _maximalDifferencePerColorChannel <
          _channelDifference(downscaledRendered.getPixel(blockX, blockY), downscaledBaseline.getPixel(blockX, blockY));
      if (differs) {
        visualization.setPixelRgb(x, y, 255, 0, 0);
      } else {
        final img.Pixel original = rendered.getPixel(x, y);
        final int gray = ((original.r + original.g + original.b) / 3 * 0.5).round();
        visualization.setPixelRgb(x, y, gray, gray, gray);
      }
    }
  }
  return visualization;
}

File _preserveFailureArtifact(String baselineName, String suffix, img.Image image) {
  final Directory directory = _failuresDirectory();
  directory.createSync(recursive: true);
  final File file = File(path.join(directory.path, '${baselineName}_$suffix.png'));
  file.writeAsBytesSync(img.encodePng(image));
  return file;
}
