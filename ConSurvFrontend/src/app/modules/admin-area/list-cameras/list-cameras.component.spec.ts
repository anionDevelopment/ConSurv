import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListCamerasComponent } from './list-cameras.component';

describe('ListCamerasComponent', () => {
  let component: ListCamerasComponent;
  let fixture: ComponentFixture<ListCamerasComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ListCamerasComponent]
    });
    fixture = TestBed.createComponent(ListCamerasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
