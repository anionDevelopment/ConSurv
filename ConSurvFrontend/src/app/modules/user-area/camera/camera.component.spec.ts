import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CameraComponent } from './camera.component';
import { FrameWorkComponent } from '../../home-page/frame-work/frame-work.component';
import { UserDataService } from '../../../services/user-data.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CameraDTO } from '../../../generated/con-surv-backend/model/cameraDTO';
import { of } from 'rxjs';
import { CameraService } from '../../../generated/con-surv-backend';
import { HomePageModule } from '../../home-page/home-page.module';

describe('CameraComponent', () => {
  let component: CameraComponent;
  let fixture: ComponentFixture<CameraComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        FrameWorkComponent,
        CameraComponent,
      ],
      imports: [
        HomePageModule,
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
            aPIV1CameraControllerCameraCameraIdGet: () => of({
              cameraId: "1",
              name: "camera",
            }),
          },
        }
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CameraComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
