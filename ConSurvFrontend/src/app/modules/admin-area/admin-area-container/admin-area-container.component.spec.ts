import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminAreaContainerComponent } from './admin-area-container.component';
import { UserService } from '../../../generated/con-surv-backend';

describe('AdminAreaContainerComponent', () => {
  let component: AdminAreaContainerComponent;
  let fixture: ComponentFixture<AdminAreaContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        AdminAreaContainerComponent,
      ],
      providers: [
        {
          provide: UserService,
          useValue: {}
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminAreaContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
