import { Component } from '@angular/core';
import { LoginFormComponent } from "../login-form/login-form.component";
import { Settings } from '../../../static/Settings';
import { UserDataService } from '../../../services/user-data.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home-page',
  standalone: false,
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss'
})
export class HomePageComponent {
  constructor(userDataService: UserDataService, router: Router) {
    if (userDataService.userIsLoggedIn()) {
      router.navigate(['user', 'dashboard']);
    }
  }
}
