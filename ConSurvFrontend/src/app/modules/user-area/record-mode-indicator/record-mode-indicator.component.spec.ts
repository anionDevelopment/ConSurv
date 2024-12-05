import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecordModeIndicatorComponent } from './record-mode-indicator.component';

describe('RecordModeIndicatorComponent', () => {
  let component: RecordModeIndicatorComponent;
  let fixture: ComponentFixture<RecordModeIndicatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RecordModeIndicatorComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(RecordModeIndicatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
