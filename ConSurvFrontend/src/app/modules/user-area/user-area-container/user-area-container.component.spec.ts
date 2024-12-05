import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAreaContainerComponent } from './user-area-container.component';
import { UserDataService } from '../../../services/user-data.service';

describe('UserAreaContainerComponent', () => {
  let component: UserAreaContainerComponent;
  let fixture: ComponentFixture<UserAreaContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        UserAreaContainerComponent,
      ],
      providers: [
        {
          provide: UserDataService,
          useValue: {}
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserAreaContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
