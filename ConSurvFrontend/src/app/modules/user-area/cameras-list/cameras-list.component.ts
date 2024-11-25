import { Component } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';

@Component({
  selector: 'app-cameras-list',
  standalone: false,
  templateUrl: './cameras-list.component.html',
  styleUrl: './cameras-list.component.scss'
})
export class CamerasListComponent {

  userIdAdmin: boolean | null = null;
  constructor(userDataService: UserDataService) {
    userDataService.userIsAdmin().subscribe((isAdmin) => {
      this.userIdAdmin = isAdmin;
    });
  }
  addCamera() {
    throw new Error('Method not implemented.');
  }
}
