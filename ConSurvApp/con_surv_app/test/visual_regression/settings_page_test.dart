import 'package:con_surv_app/app_settings.dart';
import 'package:con_surv_app/settings_page.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences_platform_interface/in_memory_shared_preferences_async.dart';
import 'package:shared_preferences_platform_interface/shared_preferences_async_platform_interface.dart';

import 'visual_regression.dart';

void main() {
  testWidgets(
    'the settings-page looks like its baseline',
    (WidgetTester tester) async {
      // Fakes the platform-side of "shared_preferences", so that AppSettings.load() does not need a real
      // platform-channel in a test (see lib/app_settings.dart).
      SharedPreferencesAsyncPlatform.instance = InMemorySharedPreferencesAsync.empty();
      final AppSettings settings = await AppSettings.load();
      await expectAppToLookLikeBaseline(
        tester,
        wrapWithApp(SettingsPage(settings: settings)),
        'settings-page',
        surfaceSize: const Size(1000, 1200),
      );
    },
    tags: visualRegressionTestTag,
  );
}
