import { Component } from '@angular/core';
import { Theme } from './modules/home-page/theme-switch/ThemeMode';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'ConSurvFrontend';

  darkThemeSelected: boolean = true;

  themeChanged(newTheme: Theme): void {
    if (newTheme == Theme.Light) {
      this.darkThemeSelected = false;
    } else if (newTheme == Theme.Dark) {
      this.darkThemeSelected = true;
    } else {
      throw Error(`Unknown theme-mode-value: ${newTheme}`);
    }
  }
}
