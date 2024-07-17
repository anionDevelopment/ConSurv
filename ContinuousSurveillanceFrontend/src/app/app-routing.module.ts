import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './modules/utilities/not-found/not-found.component';
import { HomePageComponent } from './modules/home-page/home-page/home-page.component';
import { UserHomePageComponent } from './modules/user-area/user-home-page/user-home-page.component';
import { AdminHomePageComponent } from './modules/admin-area/admin-home-page/admin-home-page.component';
import { UserProfileComponent } from './modules/utilities/user-profile/user-profile.component';

const routes: Routes = [
  { path: '', component: HomePageComponent, pathMatch: 'full' },
  { path: 'user', component: UserHomePageComponent, pathMatch: 'full' },
  { path: 'user/profile', component: UserProfileComponent, pathMatch: 'full' },
  { path: 'admin', component: AdminHomePageComponent, pathMatch: 'full' },
  { path: '**', component: NotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
