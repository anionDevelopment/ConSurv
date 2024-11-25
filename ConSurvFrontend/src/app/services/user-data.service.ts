import { Injectable } from '@angular/core';
import { UserService } from '../generated/con-surv-backend';
import { StorageService } from './storage.service';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserDataService {
  loadUserData(): Observable<void> {
    console.log("x1");
    return this.userService.aPIV1UserControllerGetUserInformationGet(this.storageService.getAccessToken()).pipe(
      tap((value: any) => {
        this.storageService.setUserName(value.name);
        this.storageService.setUserId(value.id);
        this.storageService.setUserIsAdmin(value.isAdmin);
      }
      ));
  }
  unloadUserData() {
    this.storageService.setUserName(null);
    this.storageService.setUserId(null);
    this.storageService.setUserIsAdmin(false);
  }

  constructor(private userService: UserService, private storageService: StorageService) {
  }

  getUserId(): string {
    return this.storageService.getUserId();
  }

  getUserName(): string {
    return this.storageService.getUserName();
  }

  userIsLoggedIn(): boolean {
    try {
      if (this.getUserId()) {
        return true;
      } else {
        return false;
      }
    } catch {
      return false;
    }
  }

  userIsAdmin(): boolean {
    try {
      return this.storageService.getUserIsAdmin();
    } catch {
      return false;
    }
  }
}
