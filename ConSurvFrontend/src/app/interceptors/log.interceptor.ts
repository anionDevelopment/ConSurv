import { HttpInterceptorFn } from '@angular/common/http';
import { Settings } from '../static/Settings';

export const logInterceptor: HttpInterceptorFn = (req, next) => {
  if (Settings.isVerbose()) {
    console.log(req);
  }
  return next(req);
};
