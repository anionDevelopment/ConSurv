import { Injectable } from '@angular/core';
import { ImportDeclaration } from 'typescript';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  public static readonly keyAccessToken: string = 'accessToken';
  public static readonly keyUserName: string = 'userName';
  public static readonly keyUserId: string = 'userId';
  public static readonly keyUserIsAdmin: string = 'userIsAdmin';
  constructor() { }

  public setAccessToken(value: string): void {
    sessionStorage.setItem(StorageService.keyAccessToken, value);
  }

  public getAccessToken(): string {
    const result: string | null = sessionStorage.getItem(StorageService.keyAccessToken);
    if (result) {
      return result;
    }
    else {
      throw Error("AccessToken is not available.");
    }
  }

  public removeAccessToken(): void {
    sessionStorage.removeItem(StorageService.keyAccessToken);
  }

  public setUserName(value: string | null): void {
    if (value) {
      sessionStorage.setItem(StorageService.keyUserName, value);
    } else {
      sessionStorage.removeItem(StorageService.keyUserName);
    }
  }

  public getUserName(): string {
    const result: string | null = sessionStorage.getItem(StorageService.keyUserName);
    if (result) {
      return result;
    }
    else {
      throw Error("UserName is not available.");
    }
  }

  public setUserId(value: string | null): void {
    if (value) {
      sessionStorage.setItem(StorageService.keyUserId, value);
    } else {
      sessionStorage.removeItem(StorageService.keyUserId);
    }
  }

  public getUserId(): string {
    const result: string | null = sessionStorage.getItem(StorageService.keyUserId);
    if (result) {
      return result;
    }
    else {
      throw Error("UserId is not available.");
    }
  }
  public setUserIsAdmin(value: boolean | null): void {
    if (value) {
      sessionStorage.setItem(StorageService.keyUserIsAdmin, value.toString());
    } else {
      sessionStorage.removeItem(StorageService.keyUserIsAdmin);
    }
  }

  public getUserIsAdmin(): boolean {
    const result: string | null = sessionStorage.getItem(StorageService.keyUserIsAdmin);
    if (result) {
      return (result === 'true');;
    }
    else {
      throw Error("UserIsAdmin is not available.");
    }
  }
}
