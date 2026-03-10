import { CanActivateFn, Router } from '@angular/router';
import { UserDataService } from '../services/user-data.service';
import { inject } from '@angular/core';
import { tap } from 'rxjs';

export const authenticationCheckGuard: CanActivateFn = (route, state) => {
  const userDataService: UserDataService = inject(UserDataService);
  const router: Router = inject(Router);
  return userDataService.userIsLoggedIn().pipe(tap((userIsLoggedIn) => {
    if (!userIsLoggedIn) {
      router.navigate(['/']);//TODO add some kind of query params for example to indicate on the homepage that the user is not logged in
    }
    return userIsLoggedIn;
  }));
};
