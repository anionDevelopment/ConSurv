import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCameraDialogComponent } from './edit-camera-dialog.component';

describe('EditCameraDialogComponent', () => {
  let component: EditCameraDialogComponent;
  let fixture: ComponentFixture<EditCameraDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EditCameraDialogComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(EditCameraDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
