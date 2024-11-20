import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CamerasListComponent } from './cameras-list.component';

describe('CamerasListComponent', () => {
  let component: CamerasListComponent;
  let fixture: ComponentFixture<CamerasListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CamerasListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CamerasListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
