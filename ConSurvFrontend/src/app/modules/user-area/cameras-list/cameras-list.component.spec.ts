import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CamerasListComponent } from './cameras-list.component';
import { CameraService, UserService } from '../../../generated/con-surv-backend';
import { CameraComponent } from '../camera/camera.component';
import { HomePageModule } from '../../home-page/home-page.module';
import { of } from 'rxjs';
import { UserDataService } from '../../../services/user-data.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { StorageService } from '../../../services/storage.service';
import { Router } from '@angular/router';
import { UserAreaContainerComponent } from '../user-area-container/user-area-container.component';
import { Component } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDividerModule } from '@angular/material/divider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatInputModule } from '@angular/material/input';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';

@Component({
  selector: 'app-user-area-container',
  standalone: false,
  template: '<ng-content></ng-content>'
})
class MockUserAreaContainerComponent { }

describe('CamerasListComponent', () => {
  let component: CamerasListComponent;
  let fixture: ComponentFixture<CamerasListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
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
        MatIconModule,
        MatTableModule,
        MatSidenavModule,
        MatButtonModule,
        MatTabsModule,
        HomePageModule,
      ],
      declarations: [
        MockUserAreaContainerComponent,
        CamerasListComponent,
      ],
      providers: [
        {
          provide: UserDataService,
          useValue: {
            userIsModerator: () => of(true),
          },
        },
        {
          provide: CameraService,
          useValue: {
            aPIV1CameraControllerCamerasGet: () => of([]),
          },
        },
        {
          provide: StorageService,
          useValue: {
            getAccessToken: () => "someAccessToken",
          },
        },
        {
          provide: Router,
          useValue: {
            url: "user/cameras",
          },
        },
        {
          provide: MatDialog,
          useValue: {
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CamerasListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
