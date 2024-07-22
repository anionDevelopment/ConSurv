import { Component } from '@angular/core';
import { Theme } from '../theme-switch/ThemeMode';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent {

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
