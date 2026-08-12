import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-users-list',
  standalone: false,
  templateUrl: './users-list.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './users-list.component.scss'
})
export class UsersListComponent {

}
