import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminDashboardComponent } from './admin-dashboard.component';
import { FrameWorkComponent } from '../../home-page/frame-work/frame-work.component';
import { AdminAreaContainerComponent } from '../admin-area-container/admin-area-container.component';
import { UserService } from '../../../generated/con-surv-backend';

describe('AdminDashboardComponent', () => {
  let component: AdminDashboardComponent;
  let fixture: ComponentFixture<AdminDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        FrameWorkComponent,
        AdminAreaContainerComponent,
        AdminDashboardComponent,
      ],
      providers: [
        {
          provide: UserService,
          useValue: {}
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
