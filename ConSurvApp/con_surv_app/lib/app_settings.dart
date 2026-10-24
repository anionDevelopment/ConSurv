import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

/// Holds the user-configurable settings of the app (currently only the theme-mode), persists them via
/// [SharedPreferencesAsync] - the standard way to persist small, simple values in a Flutter app - and notifies its
/// listeners whenever one of them changes, so that both the root [MaterialApp] (which applies the theme-mode) and the
/// settings-page itself react to a change without every widget needing its own copy of the state.
class AppSettings extends ChangeNotifier {
  static const String _themeModeKey = 'themeMode';

  final SharedPreferencesAsync _preferences;

  ThemeMode _themeMode = ThemeMode.system;

  AppSettings._(this._preferences);

  /// Loads the settings which were persisted by a previous run of the app, falling back to the default
  /// ([ThemeMode.system]) for whichever of them was never set yet.
  static Future<AppSettings> load() async {
    final SharedPreferencesAsync preferences = SharedPreferencesAsync();
    final AppSettings settings = AppSettings._(preferences);
    final String? themeModeName = await preferences.getString(_themeModeKey);
    settings._themeMode = ThemeMode.values.firstWhere(
      (ThemeMode mode) => mode.name == themeModeName,
      orElse: () => ThemeMode.system,
    );
    return settings;
  }

  ThemeMode get themeMode => _themeMode;

  Future<void> setThemeMode(ThemeMode value) async {
    if (_themeMode == value) {
      return;
    }
    _themeMode = value;
    notifyListeners();
    await _preferences.setString(_themeModeKey, value.name);
  }
}
