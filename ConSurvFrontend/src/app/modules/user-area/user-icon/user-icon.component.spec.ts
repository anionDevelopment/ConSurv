import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserIconComponent } from './user-icon.component';
import { UserDataService } from '../../../services/user-data.service';
import { UserService } from '../../../generated/con-surv-backend';

describe('UserIconComponent', () => {
  let component: UserIconComponent;
  let fixture: ComponentFixture<UserIconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        UserIconComponent,
      ],
      providers: [
        {
          provide: UserDataService,
          useValue: {}
        },
        {
          provide: UserService,
          useValue: {}
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserIconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
