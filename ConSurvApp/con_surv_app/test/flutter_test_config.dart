import 'dart:async';
import 'dart:io';

import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';

/// Runs before every test-file of this package.
///
/// Without this, `flutter test` renders text and icons with a placeholder test-font (solid boxes instead of glyphs)
/// instead of the "Roboto" material design uses (see "lib/app_theme.dart", which sets `fontFamily: 'Roboto'`) and the
/// "MaterialIcons"-font every `Icons.*` uses. That would make the baseline-images of the visual-regression-tests
/// (see "test/visual_regression") show boxes instead of readable text and icons, so both fonts are loaded here once
/// for the whole test-run.
///
/// Neither font-file is part of this repository: both ship inside the Flutter-SDK itself
/// (`$FLUTTER_ROOT/bin/cache/artifacts/material_fonts`, the exact files `flutter build` already bundles into every
/// real application because of pubspec.yaml's `uses-material-design: true` - `flutter test` just never loads them),
/// so they are read from there via the `FLUTTER_ROOT`-environment-variable which `flutter test` sets for the process
/// it spawns.
Future<void> testExecutable(FutureOr<void> Function() testMain) async {
  TestWidgetsFlutterBinding.ensureInitialized();
  final String? flutterRoot = Platform.environment['FLUTTER_ROOT'];
  if (flutterRoot != null) {
    final Directory fontsFolder = Directory('$flutterRoot/bin/cache/artifacts/material_fonts');
    await _loadFont('Roboto', fontsFolder, 'roboto-regular.ttf');
    await _loadFont('Roboto', fontsFolder, 'roboto-medium.ttf');
    await _loadFont('MaterialIcons', fontsFolder, 'materialicons-regular.otf');
  }
  return testMain();
}

/// Registers the font-file [fileName] of [folder] under the font-family [family], if that file exists.
///
/// The file is looked up case-insensitively because the Flutter-SDK renamed these files from "Roboto-Regular.ttf" to
/// "roboto-regular.ttf": the case therefore depends on the version of the SDK, and on Linux (which the
/// visual-regression-container runs on) a wrong case is not found at all. A file which does not exist is skipped
/// instead of failing the whole test-run: only the visual-regression-tests really depend on the real fonts, and
/// those run against a fixed image whose SDK does contain them.
Future<void> _loadFont(String family, Directory folder, String fileName) async {
  if (!folder.existsSync()) {
    return;
  }
  final String expectedFileName = fileName.toLowerCase();
  for (final FileSystemEntity entity in folder.listSync()) {
    if (entity is File && entity.uri.pathSegments.last.toLowerCase() == expectedFileName) {
      final ByteData fontData = entity.readAsBytesSync().buffer.asByteData();
      await (FontLoader(family)..addFont(Future<ByteData>.value(fontData))).load();
      return;
    }
  }
}
