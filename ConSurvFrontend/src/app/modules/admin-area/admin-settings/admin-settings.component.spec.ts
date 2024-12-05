import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminSettingsComponent } from './admin-settings.component';
import { FrameWorkComponent } from '../../home-page/frame-work/frame-work.component';
import { AdminAreaContainerComponent } from '../admin-area-container/admin-area-container.component';
import { UserService } from '../../../generated/con-surv-backend';

describe('AdminSettingsComponent', () => {
  let component: AdminSettingsComponent;
  let fixture: ComponentFixture<AdminSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        FrameWorkComponent,
        AdminAreaContainerComponent,
        AdminSettingsComponent,
      ],
      providers: [
        {
          provide: UserService,
          useValue: {}
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
