import { TestBed } from '@angular/core/testing';

import { StorageService } from './storage.service';

describe('StorageService', () => {
  let service: StorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StorageService);
    sessionStorage.clear();
  });

  afterEach(() => {
    sessionStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('access token', () => {
    it('should store a non-empty access token in sessionStorage', () => {
      service.setAccessToken('token-abc');
      expect(sessionStorage.getItem(StorageService.keyAccessToken)).toBe('token-abc');
    });

    it('should remove the access token when set to null', () => {
      sessionStorage.setItem(StorageService.keyAccessToken, 'existing');
      service.setAccessToken(null);
      expect(sessionStorage.getItem(StorageService.keyAccessToken)).toBeNull();
    });

    it('should return the stored access token', () => {
      sessionStorage.setItem(StorageService.keyAccessToken, 'token-abc');
      expect(service.getAccessToken()).toBe('token-abc');
    });

    it('should throw when getting an unset access token', () => {
      expect(() => service.getAccessToken()).toThrowError('AccessToken is not available.');
    });

    it('should report hasAccessToken=true when set', () => {
      sessionStorage.setItem(StorageService.keyAccessToken, 'token-abc');
      expect(service.hasAccessToken()).toBeTrue();
    });

    it('should report hasAccessToken=false when not set', () => {
      expect(service.hasAccessToken()).toBeFalse();
    });

    it('should remove the access token via removeAccessToken', () => {
      sessionStorage.setItem(StorageService.keyAccessToken, 'token-abc');
      service.removeAccessToken();
      expect(sessionStorage.getItem(StorageService.keyAccessToken)).toBeNull();
    });
  });

  describe('user name', () => {
    it('should store the user name', () => {
      service.setUserName('alice');
      expect(sessionStorage.getItem(StorageService.keyUserName)).toBe('alice');
    });

    it('should remove the user name when set to null', () => {
      sessionStorage.setItem(StorageService.keyUserName, 'alice');
      service.setUserName(null);
      expect(sessionStorage.getItem(StorageService.keyUserName)).toBeNull();
    });

    it('should remove the user name when set to undefined', () => {
      sessionStorage.setItem(StorageService.keyUserName, 'alice');
      service.setUserName(undefined);
      expect(sessionStorage.getItem(StorageService.keyUserName)).toBeNull();
    });

    it('should return the stored user name', () => {
      sessionStorage.setItem(StorageService.keyUserName, 'alice');
      expect(service.getUserName()).toBe('alice');
    });

    it('should throw when getting an unset user name', () => {
      expect(() => service.getUserName()).toThrowError('UserName is not available.');
    });
  });

  describe('user id', () => {
    it('should store the user id', () => {
      service.setUserId('user-42');
      expect(sessionStorage.getItem(StorageService.keyUserId)).toBe('user-42');
    });

    it('should remove the user id when set to null', () => {
      sessionStorage.setItem(StorageService.keyUserId, 'user-42');
      service.setUserId(null);
      expect(sessionStorage.getItem(StorageService.keyUserId)).toBeNull();
    });

    it('should return the stored user id', () => {
      sessionStorage.setItem(StorageService.keyUserId, 'user-42');
      expect(service.getUserId()).toBe('user-42');
    });

    it('should throw when getting an unset user id', () => {
      expect(() => service.getUserId()).toThrowError('UserId is not available.');
    });
  });

  describe('user is admin', () => {
    it('should store true as the string "true"', () => {
      service.setUserIsAdmin(true);
      expect(sessionStorage.getItem(StorageService.keyUserIsAdmin)).toBe('true');
    });

    it('should store false as the string "false"', () => {
      service.setUserIsAdmin(false);
      expect(sessionStorage.getItem(StorageService.keyUserIsAdmin)).toBe('false');
    });

    it('should remove the flag when set to null', () => {
      sessionStorage.setItem(StorageService.keyUserIsAdmin, 'true');
      service.setUserIsAdmin(null);
      expect(sessionStorage.getItem(StorageService.keyUserIsAdmin)).toBeNull();
    });

    it('should remove the flag when set to undefined', () => {
      sessionStorage.setItem(StorageService.keyUserIsAdmin, 'true');
      service.setUserIsAdmin(undefined);
      expect(sessionStorage.getItem(StorageService.keyUserIsAdmin)).toBeNull();
    });

    it('should return true when the stored value is "true"', () => {
      sessionStorage.setItem(StorageService.keyUserIsAdmin, 'true');
      expect(service.getUserIsAdmin()).toBeTrue();
    });

    it('should throw when getting an unset admin flag', () => {
      expect(() => service.getUserIsAdmin()).toThrowError('UserIsAdmin is not available.');
    });
  });

  describe('user is moderator', () => {
    it('should store true as the string "true"', () => {
      service.setUserIsModerator(true);
      expect(sessionStorage.getItem(StorageService.keyUserIsModerator)).toBe('true');
    });

    it('should store false as the string "false"', () => {
      service.setUserIsModerator(false);
      expect(sessionStorage.getItem(StorageService.keyUserIsModerator)).toBe('false');
    });

    it('should remove the flag when set to null', () => {
      sessionStorage.setItem(StorageService.keyUserIsModerator, 'true');
      service.setUserIsModerator(null);
      expect(sessionStorage.getItem(StorageService.keyUserIsModerator)).toBeNull();
    });

    it('should return true when the stored value is "true"', () => {
      sessionStorage.setItem(StorageService.keyUserIsModerator, 'true');
      expect(service.getUserIsModerator()).toBeTrue();
    });

    it('should throw when getting an unset moderator flag', () => {
      expect(() => service.getUserIsModerator()).toThrowError('UserIsModerator is not available.');
    });
  });
});
