import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCameraDialogComponent } from './edit-camera-dialog.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDividerModule } from '@angular/material/divider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatInputModule } from '@angular/material/input';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { HomePageModule } from '../../home-page/home-page.module';
import { CameraService } from '../../../generated/con-surv-backend';
import { RecordModeIndicatorComponent } from '../record-mode-indicator/record-mode-indicator.component';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

describe('EditCameraDialogComponent', () => {
  let component: EditCameraDialogComponent;
  let fixture: ComponentFixture<EditCameraDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        CommonModule,
        NoopAnimationsModule,
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
        RecordModeIndicatorComponent,
        EditCameraDialogComponent,
      ],
      providers: [
        {
          provide: MatDialogRef,
          useValue: {}
        },
        {
          provide: CameraService,
          useValue: {},
        },
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            camera: {
              name: "cameraname",
              videoInformationDTO: {
                streamURL: "rtsp://example.com/stream",
                supportsPTZViaONVIF: true,
              },
              recordModeDTO: {
                recordMode: "NoRecording"
              },
            },
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(EditCameraDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
