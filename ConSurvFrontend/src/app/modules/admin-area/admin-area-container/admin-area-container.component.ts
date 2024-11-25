import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserDataService } from '../../../services/user-data.service';

@Component({
  selector: 'app-admin-area-container',
  standalone: false,
  templateUrl: './admin-area-container.component.html',
  styleUrl: './admin-area-container.component.scss'
})
export class AdminAreaContainerComponent {
  activeSite: string;
  userIsAdmin: boolean | null = null;
  constructor(private router: Router, userDataService: UserDataService) {
    this.activeSite = this.getSiteTitle(this.router.url.split('/').pop()!);
    userDataService.userIsAdmin().subscribe((isAdmin => {
      this.userIsAdmin = isAdmin;
    }));
  }
  onDashboardClick() {
    this.router.navigate(['admin', 'dashboard']);
  }
  onCamerasClick() {
    this.router.navigate(['admin', 'users']);
  }
  onSettingsClick() {
    this.router.navigate(['admin', 'settings']);
  }
  onUserAreaClick() {
    this.router.navigate(['user', 'dashboard']);
  }
  getSiteTitle(urlSegment: string): string {
    switch (urlSegment) {
      case "admin": {
        return "Admin-Area";
      }
      case "dashboard": {
        return "Dashboard";
      }
      case "users": {
        return "Users";
      }
      case "settings": {
        return "Settings";
      }
      case "user": {
        return "User-Area";
      }
      default: {
        throw new Error('Unknown urlSegment: ' + urlSegment);
      }
    }
  }
}
