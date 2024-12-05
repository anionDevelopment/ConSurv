import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CamerasListComponent } from './cameras-list.component';
import { UserService } from '../../../generated/con-surv-backend';
import { FooterComponent } from '../../home-page/footer/footer.component';

describe('CamerasListComponent', () => {
  let component: CamerasListComponent;
  let fixture: ComponentFixture<CamerasListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        FooterComponent,
        CamerasListComponent,
      ],
      providers: [
        {
          provide: UserService,
          useValue: {}
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CamerasListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
