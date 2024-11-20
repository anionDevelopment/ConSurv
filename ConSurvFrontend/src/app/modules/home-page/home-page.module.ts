import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomePageComponent } from './home-page/home-page.component';
import { LoginFieldComponent } from './login-field/login-field.component';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { ThemeSwitchComponent } from './theme-switch/theme-switch.component';
import { BackendURLUpdaterInterceptor } from 'src/app/interceptors/backend-urlupdater.interceptor';

@NgModule({
  declarations: [
    HomePageComponent,
    LoginFieldComponent,
    ThemeSwitchComponent,
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    MatButtonToggleModule,
    MatInputModule,
    MatCardModule,
    MatIconModule,
    FormsModule,
    MatButtonModule,
    BrowserModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
  ],
  exports: [
    HomePageComponent,
    ThemeSwitchComponent,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: BackendURLUpdaterInterceptor,
      multi: true,
    }
  ],
})
export class HomePageModule { }
