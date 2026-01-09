import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CameraPreviewComponent } from './camera-preview.component';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { CameraService } from '../../../generated/con-surv-backend';

describe('CameraPreviewComponent', () => {
  let component: CameraPreviewComponent;
  let fixture: ComponentFixture<CameraPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
      ],
      imports: [
      ],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of({
              "cameraId": "1"
            }),
          },
        },
        {
          provide: CameraService,
          useValue: {
            aPIV3CameraControllerCameraCameraIdGet: () => of({
              cameraId: "1",
              name: "camera",
            }),
          },
        }
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CameraPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
