import { TestBed } from '@angular/core/testing';

import { BackendURLUpdaterInterceptor } from './backend-urlupdater.interceptor';

describe('BackendURLUpdaterInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      BackendURLUpdaterInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: BackendURLUpdaterInterceptor = TestBed.inject(BackendURLUpdaterInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
