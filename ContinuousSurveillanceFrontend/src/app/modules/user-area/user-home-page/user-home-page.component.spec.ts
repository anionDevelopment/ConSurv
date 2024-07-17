import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from "@angular/router/testing";
import { UserHomePageComponent } from './user-home-page.component';
import { UtilitiesModule } from '../../utilities/utilities.module';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatListModule } from '@angular/material/list';
import { UserDashboardComponent } from '../user-dashboard/user-dashboard.component';

describe('UserHomePageComponent', () => {
  let component: UserHomePageComponent;
  let fixture: ComponentFixture<UserHomePageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        UtilitiesModule,
        RouterTestingModule,
        MatToolbarModule,
        MatIconModule,
        MatSidenavModule,
        NoopAnimationsModule,
        MatListModule,
      ],
      declarations: [
        UserDashboardComponent,
        UserHomePageComponent,
      ]
    });
    fixture = TestBed.createComponent(UserHomePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
