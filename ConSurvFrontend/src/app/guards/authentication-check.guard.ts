import { CanActivateFn } from '@angular/router';

export const authenticationCheckGuard: CanActivateFn = (route, state) => {
  //TODO check if the user is authenticated and show error if not
  return true;
};
