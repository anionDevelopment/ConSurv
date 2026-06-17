import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Settings } from '../static/Settings';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {

  constructor() { }
  public isProductiveServer(): boolean {
    if (environment.production) {
      return environment.production!;
    } else {
      throw new Error("environment.production is not set.");
    }
  }

  public getAPIURL(): string {
    return Settings.getAPIUrl();
  }
}
