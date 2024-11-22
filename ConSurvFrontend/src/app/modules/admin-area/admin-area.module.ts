import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AdminSettingsComponent } from './admin-settings/admin-settings.component';
import { UserComponent } from './user/user.component';
import { UsersListComponent } from './users-list/users-list.component';



@NgModule({
  declarations: [
    AdminDashboardComponent,
    AdminSettingsComponent,
    UserComponent,
    UsersListComponent,
  ],
  imports: [
    CommonModule,
  ]
})
export class AdminAreaModule { }
