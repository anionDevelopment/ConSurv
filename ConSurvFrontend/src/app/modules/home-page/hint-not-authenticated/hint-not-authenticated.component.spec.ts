import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HintNotAuthenticatedComponent } from './hint-not-authenticated.component';

describe('HintNotAuthenticatedComponent', () => {
  let component: HintNotAuthenticatedComponent;
  let fixture: ComponentFixture<HintNotAuthenticatedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HintNotAuthenticatedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HintNotAuthenticatedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
