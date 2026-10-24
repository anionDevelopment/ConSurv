import 'package:flutter/material.dart';

import 'app_settings.dart';

/// The settings-page of the app. Currently it offers a single setting: the theme-mode, chosen between following the
/// system, light and dark (see [AppSettings.setThemeMode]). It rebuilds itself whenever the settings change (via the
/// [ListenableBuilder] on [settings]), so the selected radio-option reflects the current value immediately.
class SettingsPage extends StatelessWidget {
  const SettingsPage({super.key, required this.settings});

  final AppSettings settings;

  @override
  Widget build(BuildContext context) {
    return ListenableBuilder(
      listenable: settings,
      builder: (BuildContext context, Widget? child) {
        return Scaffold(
          appBar: AppBar(title: const Text('Settings')),
          // Keeps the content out of the area the system draws its own bars into (the app is drawn edge-to-edge,
          // which is what Flutter does by default and what recent Android-versions enforce).
          body: SafeArea(
            child: ListView(
              children: <Widget>[
                Padding(
                  padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
                  child: Text('Theme', style: Theme.of(context).textTheme.titleMedium),
                ),
                RadioGroup<ThemeMode>(
                  groupValue: settings.themeMode,
                  onChanged: (ThemeMode? value) => settings.setThemeMode(value ?? ThemeMode.system),
                  child: const Column(
                    children: <Widget>[
                      RadioListTile<ThemeMode>(title: Text('Follow system'), value: ThemeMode.system),
                      RadioListTile<ThemeMode>(title: Text('Light'), value: ThemeMode.light),
                      RadioListTile<ThemeMode>(title: Text('Dark'), value: ThemeMode.dark),
                    ],
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
