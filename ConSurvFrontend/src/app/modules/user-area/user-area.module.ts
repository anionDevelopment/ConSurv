import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CameraComponent } from './camera/camera.component';
import { UserDashboardComponent } from './user-dashboard/user-dashboard.component';
import { UserSettingsComponent } from './user-settings/user-settings.component';
import { CamerasListComponent } from './cameras-list/cameras-list.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { UserAreaContainerComponent } from './user-area-container/user-area-container.component';
import { MatDividerModule } from '@angular/material/divider';
import { HomePageModule } from '../home-page/home-page.module';
import { MatIconModule } from '@angular/material/icon';
import { UserIconComponent } from './user-icon/user-icon.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { RecordModeIndicatorComponent } from "./record-mode-indicator/record-mode-indicator.component";
import { RecordStateIndicatorComponent } from './record-state-indicator/record-state-indicator.component';
import { MatDialogModule } from '@angular/material/dialog';
import { EditCameraDialogComponent } from './edit-camera-dialog/edit-camera-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';



@NgModule({
  declarations: [
    CameraComponent,
    CamerasListComponent,
    EditCameraDialogComponent,
    UserIconComponent,
    UserAreaContainerComponent,
    UserDashboardComponent,
    UserSettingsComponent,
    RecordModeIndicatorComponent,
    RecordStateIndicatorComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatFormFieldModule,
    MatDividerModule,
    MatCheckboxModule,
    MatSelectModule,
    MatMenuModule,
    MatTooltipModule,
    MatInputModule,
    MatDialogModule,
    HomePageModule,
    MatIconModule,
    MatTableModule,
    MatSidenavModule,
    MatButtonModule,
    MatTabsModule,
  ]
})
export class UserAreaModule { }
