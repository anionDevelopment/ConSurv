import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserMenuComponent } from './user-menu.component';
import { MatIconModule } from '@angular/material/icon';

describe('UserMenuComponent', () => {
  let component: UserMenuComponent;
  let fixture: ComponentFixture<UserMenuComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports:[
        MatIconModule,
      ],
      declarations: [
        UserMenuComponent,
      ]
    });
    fixture = TestBed.createComponent(UserMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
