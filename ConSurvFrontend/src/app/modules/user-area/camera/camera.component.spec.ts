import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CameraComponent } from './camera.component';
import { FrameWorkComponent } from '../../home-page/frame-work/frame-work.component';
import { UserDataService } from '../../../services/user-data.service';

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
        {
          provide: UserDataService,
          useValue: {}
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
