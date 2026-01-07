import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CameraComponent } from './camera.component';
import { UserDataService } from '../../../services/user-data.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { CameraService, StreamingService, UserService } from '../../../generated/con-surv-backend';
import { Component } from '@angular/core';
import { StorageService } from '../../../services/storage.service';

@Component({
  selector: 'app-user-area-container',
  standalone: false,
  template: '<ng-content></ng-content>'
})
class MockUserAreaContainerComponent { }
describe('CameraComponent', () => {
  let component: CameraComponent;
  let fixture: ComponentFixture<CameraComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        MockUserAreaContainerComponent,
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
          }
        },
        {
          provide: StreamingService,
          useValue: {}
        },
        {
          provide: UserDataService,
          useValue: {
            getUserName: () => of("username"),
            getUserId: () => of("1"),
            userIsAdmin: () => of(true),
          }
        },
        {
          provide: UserService,
          useValue: {
          }
        },
        {
          provide: Router,
          useValue: {
            url: "user/camera",
          },
        },
        {
          provide: StorageService,
          useValue: {
            getAccessToken: () => "at1",
          }
        },
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
