import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatListModule } from '@angular/material/list';
import { RouterTestingModule } from "@angular/router/testing";
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { UtilitiesModule } from '../../utilities/utilities.module';
import { MatSidenavModule } from '@angular/material/sidenav';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { AdminDashboardComponent } from './admin-dashboard.component';

describe('AdminDashboardComponent', () => {
  let component: AdminDashboardComponent;
  let fixture: ComponentFixture<AdminDashboardComponent>;

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
        AdminDashboardComponent
      ]
    });
    fixture = TestBed.createComponent(AdminDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
