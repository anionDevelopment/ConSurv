import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminHomePageComponent } from './admin-home-page.component';
import { MatListModule } from '@angular/material/list';
import { RouterTestingModule } from "@angular/router/testing";
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { UtilitiesModule } from '../../utilities/utilities.module';
import { MatSidenavModule } from '@angular/material/sidenav';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { AdminDashboardComponent } from '../admin-dashboard/admin-dashboard.component';

describe('AdminHomePageComponent', () => {
  let component: AdminHomePageComponent;
  let fixture: ComponentFixture<AdminHomePageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MatToolbarModule,
        MatIconModule,
        MatSidenavModule,
        UtilitiesModule,
        NoopAnimationsModule,
        MatListModule,
      ],
      declarations: [
        AdminDashboardComponent,
        AdminHomePageComponent,
      ]
    });
    fixture = TestBed.createComponent(AdminHomePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
