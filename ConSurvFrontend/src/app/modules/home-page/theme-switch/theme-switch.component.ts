import { Component, EventEmitter, Output } from '@angular/core';
import { Theme, ThemeMode } from './ThemeMode';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-theme-switch',
  templateUrl: './theme-switch.component.html',
  styleUrl: './theme-switch.component.scss'
})
export class ThemeSwitchComponent {

  public ThemeMode = ThemeMode;
  selectedMode$: BehaviorSubject<ThemeMode> = new BehaviorSubject<ThemeMode>(ThemeMode.System);
  private lastEmittedValue: Theme;
  @Output()
  public activatedTheme: EventEmitter<Theme> = new EventEmitter<Theme>();

  constructor() {
    const initialMode: ThemeMode = ThemeMode.System;
    this.selectedMode$ = new BehaviorSubject<ThemeMode>(initialMode);
    this.lastEmittedValue = this.getThemeFromThemeMode(initialMode);
    this.activatedTheme.emit(this.lastEmittedValue);
    this.selectedMode$.subscribe((newThemeMode: ThemeMode) => {
      const newTheme = this.getThemeFromThemeMode(newThemeMode);
      // console.log("new:" + newTheme);
      if (newTheme != this.lastEmittedValue) {
        this.lastEmittedValue = newTheme;
        this.activatedTheme.emit(this.lastEmittedValue);
      }
    });
  }
  private getThemeFromThemeMode(themeMode: ThemeMode): Theme {
    if (themeMode == ThemeMode.Light) {
      return Theme.Light;
    } else if (themeMode == ThemeMode.Dark) {
      return Theme.Dark;
    } else if (themeMode == ThemeMode.System) {
      return this.getSystemTheme();
    } else {
      throw Error(`Unknown theme-mode-value: ${themeMode}`);
    }
  }

  private getSystemTheme(): Theme {
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
      return Theme.Dark;
    } else {
      return Theme.Light;
    }
  }
}
