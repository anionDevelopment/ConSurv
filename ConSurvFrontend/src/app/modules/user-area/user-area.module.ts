import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserHomePageComponent } from './user-home-page/user-home-page.component';
import { ViewCamerasComponent } from './view-cameras/view-cameras.component';
import { UtilitiesModule } from '../utilities/utilities.module';
import { ViewCameraComponent } from './view-camera/view-camera.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { RouterModule } from '@angular/router';
import { UserDashboardComponent } from './user-dashboard/user-dashboard.component';

@NgModule({
  declarations: [
    UserHomePageComponent,
    ViewCameraComponent,
    ViewCamerasComponent,
    UserDashboardComponent,
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
    UserHomePageComponent
  ],
})
export class UserAreaModule { }
