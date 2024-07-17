import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomePageModule } from './modules/home-page/home-page.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { UtilitiesModule } from './modules/utilities/utilities.module';
import { AdminAreaModule } from './modules/admin-area/admin-area.module';
import { UserAreaModule } from './modules/user-area/user-area.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HomePageModule,
    UtilitiesModule,
    AdminAreaModule,
    UserAreaModule,
    BrowserAnimationsModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
