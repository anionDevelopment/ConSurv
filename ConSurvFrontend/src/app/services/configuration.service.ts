import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

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
    if (environment.apiUrl) {
      return environment.apiUrl!;
    } else {
      throw new Error("environment.apiUrl is not set.");
    }
  }
}
