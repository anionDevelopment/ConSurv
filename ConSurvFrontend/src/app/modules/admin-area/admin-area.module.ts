import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminHomePageComponent } from './admin-home-page/admin-home-page.component';
import { UtilitiesModule } from '../utilities/utilities.module';
import { ListCamerasComponent } from './list-cameras/list-cameras.component';
import { CreateCameraComponent } from './create-camera/create-camera.component';
import { EditCameraComponent } from './edit-camera/edit-camera.component';
import { ListUsersComponent } from './list-users/list-users.component';
import { EditUserComponent } from './edit-user/edit-user.component';
import { CreateUserComponent } from './create-user/create-user.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { RouterModule } from '@angular/router';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';

@NgModule({
  declarations: [
    AdminHomePageComponent,
    ListCamerasComponent,
    CreateCameraComponent,
    EditCameraComponent,
    ListUsersComponent,
    EditUserComponent,
    CreateUserComponent,
    AdminDashboardComponent,
  ],
  imports: [
    CommonModule,
    UtilitiesModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatSidenavModule,
    MatListModule,
    RouterModule,
  ],
  exports: [
    AdminHomePageComponent,
  ],
})
export class AdminAreaModule { }
