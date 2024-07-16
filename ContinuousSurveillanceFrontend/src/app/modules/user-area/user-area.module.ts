import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserHomePageComponent } from './user-home-page/user-home-page.component';



@NgModule({
  declarations: [
    UserHomePageComponent
  ],
  imports: [
    CommonModule
  ],
  exports:[
    UserHomePageComponent
  ],
})
export class UserAreaModule { }
