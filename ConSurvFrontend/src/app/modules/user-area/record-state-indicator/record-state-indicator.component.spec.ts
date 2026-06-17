import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecordStateIndicatorComponent } from './record-state-indicator.component';
import { MatIconModule } from '@angular/material/icon';

describe('RecordStateIndicatorComponent', () => {
  let component: RecordStateIndicatorComponent;
  let fixture: ComponentFixture<RecordStateIndicatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        MatIconModule,
      ],
      declarations: [
        RecordStateIndicatorComponent,
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RecordStateIndicatorComponent);
    component = fixture.componentInstance;
    component.recordState = {
      recordState: "Idle",
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
