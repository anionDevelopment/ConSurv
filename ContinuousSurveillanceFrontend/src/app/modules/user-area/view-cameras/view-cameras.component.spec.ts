import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ViewCamerasComponent } from './view-cameras.component';
import { UtilitiesModule } from '../../utilities/utilities.module';

describe('ViewCamerasComponent', () => {
  let component: ViewCamerasComponent;
  let fixture: ComponentFixture<ViewCamerasComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        UtilitiesModule,
      ],
      declarations: [
        ViewCamerasComponent,
      ]
    });
    fixture = TestBed.createComponent(ViewCamerasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
