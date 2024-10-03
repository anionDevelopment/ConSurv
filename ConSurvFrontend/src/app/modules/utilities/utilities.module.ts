import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotFoundComponent } from './not-found/not-found.component';
import { SingleViewComponent } from './single-view/single-view.component';
import { MultiViewComponent } from './multi-view/multi-view.component';
import { UserMenuComponent } from './user-menu/user-menu.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { UserProfileComponent } from './user-profile/user-profile.component';

@NgModule({
  declarations: [
    NotFoundComponent,
    SingleViewComponent,
    MultiViewComponent,
    UserMenuComponent,
    UserProfileComponent,
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
  ],
  exports: [
    NotFoundComponent,
    SingleViewComponent,
    MultiViewComponent,
    UserMenuComponent,
  ]
})
export class UtilitiesModule { }
