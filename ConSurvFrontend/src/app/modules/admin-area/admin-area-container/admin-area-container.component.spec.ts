import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminAreaContainerComponent } from './admin-area-container.component';

describe('AdminAreaContainerComponent', () => {
  let component: AdminAreaContainerComponent;
  let fixture: ComponentFixture<AdminAreaContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminAreaContainerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminAreaContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
