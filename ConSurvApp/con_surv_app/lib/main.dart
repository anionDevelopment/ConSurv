import 'package:flutter/material.dart';

import 'app_settings.dart';
import 'app_theme.dart';
import 'settings_page.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final AppSettings settings = await AppSettings.load();
  runApp(ConSurvApp(settings: settings));
}

/// The app itself. It rebuilds (via the [ListenableBuilder] on [settings]) whenever a setting changes, so switching
/// the theme-mode on the settings-page re-applies the theme to the whole app immediately.
///
/// On Android the app is drawn edge-to-edge (below the status- and the navigation-bar), which is what Flutter does by
/// default; the content of every page is kept out of those areas by a [SafeArea] around the body of its [Scaffold].
class ConSurvApp extends StatelessWidget {
  const ConSurvApp({super.key, required this.settings});

  final AppSettings settings;

  @override
  Widget build(BuildContext context) {
    return ListenableBuilder(
      listenable: settings,
      builder: (BuildContext context, Widget? child) {
        return MaterialApp(
          title: 'ConSurv',
          theme: buildThemeOfTheApp(Brightness.light),
          darkTheme: buildThemeOfTheApp(Brightness.dark),
          themeMode: settings.themeMode,
          home: HomePage(settings: settings),
        );
      },
    );
  }
}

class HomePage extends StatelessWidget {
  const HomePage({super.key, required this.settings});

  final AppSettings settings;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('ConSurv')),
      body: SafeArea(
        child: Center(
          child: ElevatedButton(
            onPressed: () {
              Navigator.of(context).push(
                MaterialPageRoute<void>(
                  builder: (BuildContext context) => SettingsPage(settings: settings),
                ),
              );
            },
            child: const Text('Settings'),
          ),
        ),
      ),
    );
  }
}
