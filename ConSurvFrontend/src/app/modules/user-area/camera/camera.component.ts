import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CameraDTO, CameraService } from '../../../generated/con-surv-backend';
import { Observable, of, switchMap } from 'rxjs';
import { StorageService } from '../../../services/storage.service';

@Component({
  selector: 'app-camera',
  standalone: false,
  templateUrl: './camera.component.html',
  styleUrl: './camera.component.scss'
})
export class CameraComponent {
  currentCamera$: Observable<CameraDTO | null>;
  constructor(route: ActivatedRoute, cameraService: CameraService, storgeService: StorageService) {
    this.currentCamera$ = route.queryParams.pipe(switchMap(params => {
      if (params["cameraId"]) {
        return cameraService.aPIV1CameraControllerCameraCameraIdGet(params["cameraId"], storgeService.getAccessToken());
      } else {
        return of(null);
      }
    }));
  }
}
