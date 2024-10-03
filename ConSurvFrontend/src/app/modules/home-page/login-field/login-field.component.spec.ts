import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginFieldComponent } from './login-field.component';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('LoginFieldComponent', () => {
  let component: LoginFieldComponent;
  let fixture: ComponentFixture<LoginFieldComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        MatCardModule,
        MatFormFieldModule,
        MatIconModule,
        ReactiveFormsModule,
        MatInputModule,
        NoopAnimationsModule,
      ],
      declarations: [
        LoginFieldComponent,
      ],
    });
    fixture = TestBed.createComponent(LoginFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
