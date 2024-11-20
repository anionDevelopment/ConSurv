import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CameraComponent } from './camera/camera.component';
import { UserDashboardComponent } from './user-dashboard/user-dashboard.component';
import { UserSettingsComponent } from './user-settings/user-settings.component';
import { CamerasListComponent } from './cameras-list/cameras-list.component';



@NgModule({
  declarations: [
    CameraComponent,
    CamerasListComponent,
    UserDashboardComponent,
    UserSettingsComponent,
  ],
  imports: [
    CommonModule,
  ]
})
export class UserAreaModule { }
