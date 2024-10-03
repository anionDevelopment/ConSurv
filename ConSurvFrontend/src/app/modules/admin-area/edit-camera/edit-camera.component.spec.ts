import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCameraComponent } from './edit-camera.component';

describe('EditCameraComponent', () => {
  let component: EditCameraComponent;
  let fixture: ComponentFixture<EditCameraComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [EditCameraComponent]
    });
    fixture = TestBed.createComponent(EditCameraComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
