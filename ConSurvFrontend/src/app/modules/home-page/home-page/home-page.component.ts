import { Component } from '@angular/core';
import { LoginFormComponent } from "../login-form/login-form.component";
import { Settings } from '../../../static/Settings';

@Component({
  selector: 'app-home-page',
  standalone: false,
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss'
})
export class HomePageComponent {
  title: string;
  constructor() {
    this.title = Settings.getAppName();
  }
}
