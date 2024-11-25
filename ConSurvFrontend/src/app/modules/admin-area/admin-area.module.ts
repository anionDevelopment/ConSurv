import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AdminSettingsComponent } from './admin-settings/admin-settings.component';
import { UserComponent } from './user/user.component';
import { UsersListComponent } from './users-list/users-list.component';
import { AdminAreaContainerComponent } from './admin-area-container/admin-area-container.component';
import { HomePageModule } from '../home-page/home-page.module';
import { UserAreaModule } from '../user-area/user-area.module';
import { FrameWorkComponent } from "../home-page/frame-work/frame-work.component";



@NgModule({
  declarations: [
    AdminAreaContainerComponent,
    AdminDashboardComponent,
    AdminSettingsComponent,
    UserComponent,
    UsersListComponent,
  ],
  imports: [
    CommonModule,
    HomePageModule,
    UserAreaModule,
  ]
})
export class AdminAreaModule { }
