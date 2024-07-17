import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ViewCameraComponent } from './view-camera.component';
import { UtilitiesModule } from '../../utilities/utilities.module';

describe('ViewCameraComponent', () => {
  let component: ViewCameraComponent;
  let fixture: ComponentFixture<ViewCameraComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports:[
        UtilitiesModule,
      ],
      declarations: [
        ViewCameraComponent,
      ]
    });
    fixture = TestBed.createComponent(ViewCameraComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
