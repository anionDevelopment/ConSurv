import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { isDevMode } from '@angular/core';

@Injectable()
export class BackendURLUpdaterInterceptor implements HttpInterceptor {

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (isDevMode()) {
      console.log(">devmode");
    } else {
      console.log(">prodmode");
    }
    const apiReq = request.clone({ url: `${request.url}` });
    return next.handle(apiReq);
  }
}
