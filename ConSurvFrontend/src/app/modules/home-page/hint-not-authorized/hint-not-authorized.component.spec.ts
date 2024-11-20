import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HintNotAuthorizedComponent } from './hint-not-authorized.component';

describe('HintNotAuthorizedComponent', () => {
  let component: HintNotAuthorizedComponent;
  let fixture: ComponentFixture<HintNotAuthorizedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HintNotAuthorizedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HintNotAuthorizedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
