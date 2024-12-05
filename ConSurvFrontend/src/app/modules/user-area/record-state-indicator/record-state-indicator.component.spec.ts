import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecordStateIndicatorComponent } from './record-state-indicator.component';

describe('RecordStateIndicatorComponent', () => {
  let component: RecordStateIndicatorComponent;
  let fixture: ComponentFixture<RecordStateIndicatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RecordStateIndicatorComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(RecordStateIndicatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
