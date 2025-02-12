import { ChangeDetectionStrategy, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
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


    const socket = new WebSocket('ws://localhost:5000/api/WebSocket/videoStream');

    socket.onopen = function () {
      console.log('WebSocket verbunden');
    };

    socket.onmessage = function (event) {
      const data = event.data; // Die empfangenen Bytes (Video-Frame)

      // Hier kannst du den Frame in einem <video> Tag oder Canvas anzeigen
      const blob = new Blob([data], { type: 'video/jpeg' });
      const url = URL.createObjectURL(blob);
      (document.getElementById('videoElement') as HTMLVideoElement).src = url;
    };

    socket.onerror = function (error) {
      console.error('WebSocket Fehler: ', error);
    };

    socket.onclose = function () {
      console.log('WebSocket getrennt');
    };
  }
}
