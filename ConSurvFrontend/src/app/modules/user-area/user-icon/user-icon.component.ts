import { Component, Input, OnInit } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { UserService } from '../../../generated/con-surv-backend';
import { StorageService } from '../../../services/storage.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-icon',
  standalone: false,
  templateUrl: './user-icon.component.html',
  styleUrl: './user-icon.component.scss'
})
export class UserIconComponent {
  userId: string;
  userName: string;
  constructor(private userDataService: UserDataService, private storageService: StorageService, private router: Router, private userService: UserService) {
    this.userId = userDataService.getUserId();
    this.userName = userDataService.getUserName();
  }
  logout() {
    this.userService.aPIV1UserControllerLogoutPut(this.storageService.getAccessToken()).subscribe(() => {
      this.userDataService.unloadUserData();
      this.router.navigate(['']);
    });
  }
}
