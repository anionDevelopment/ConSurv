import 'package:con_surv_app/app_settings.dart';
import 'package:con_surv_app/main.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences_platform_interface/in_memory_shared_preferences_async.dart';
import 'package:shared_preferences_platform_interface/shared_preferences_async_platform_interface.dart';

void main() {
  testWidgets('opens the settings-page with the theme-switch', (WidgetTester tester) async {
    // Fakes the platform-side of "shared_preferences", so that AppSettings.load() does not need a real
    // platform-channel in a test (see lib/app_settings.dart).
    SharedPreferencesAsyncPlatform.instance = InMemorySharedPreferencesAsync.empty();
    final AppSettings settings = await AppSettings.load();

    await tester.pumpWidget(ConSurvApp(settings: settings));
    expect(find.text('ConSurv'), findsOneWidget);

    await tester.tap(find.text('Settings'));
    await tester.pumpAndSettle();

    expect(find.text('Theme'), findsOneWidget);
    expect(find.text('Follow system'), findsOneWidget);
    expect(find.text('Light'), findsOneWidget);
    expect(find.text('Dark'), findsOneWidget);
  });
}
