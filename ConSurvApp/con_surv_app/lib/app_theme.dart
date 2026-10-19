import 'package:flutter/material.dart';

/// The seed the app's color-scheme is derived from. Both theme-modes use the same seed so the light- and the
/// dark-appearance stay consistent; Material derives all other color-roles from it (see [buildThemeOfTheApp]).
const Color _seedColor = Color(0xFF0D47A1);

/// Builds the theme of the app for the given [brightness] (light or dark).
///
/// Built here (instead of inline in the root [MaterialApp]) so that a test which renders a single page or widget
/// instead of the whole app can render it with the same theme the app uses. The font-family is set explicitly to
/// "Roboto" because the visual-regression-tests load exactly that font (see "test/flutter_test_config.dart"): without
/// a named family, `flutter test` would render text with a placeholder test-font (solid boxes instead of glyphs) and
/// a baseline generated out of that would be worthless.
ThemeData buildThemeOfTheApp(Brightness brightness) {
  return ThemeData(
    useMaterial3: true,
    fontFamily: 'Roboto',
    colorScheme: ColorScheme.fromSeed(seedColor: _seedColor, brightness: brightness),
  );
}
