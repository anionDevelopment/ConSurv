import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CameraDTO, CameraService, StreamingService } from '../../../generated/con-surv-backend';
import { distinctUntilChanged, filter, map, Observable, of, Subject, switchMap, takeUntil, tap } from 'rxjs';
import { StorageService } from '../../../services/storage.service';
import videojs from 'video.js';
import { ConfigurationService } from '../../../services/configuration.service';
import Player from 'video.js/dist/types/player';

@Component({
  selector: 'app-camera',
  standalone: false,
  templateUrl: './camera.component.html',
  styleUrl: './camera.component.scss'
})
export class CameraComponent implements OnInit {

  private destroy$ = new Subject<void>();
  private player: Player | null = null;
  constructor(private activatedRoute: ActivatedRoute, private cameraService: CameraService, private storgeService: StorageService, private streamingService: StreamingService, private configurationService: ConfigurationService) {
  }

  ngOnInit(): void {
    console.log("0");
    this.activatedRoute.queryParams.pipe(
      switchMap(params => {
        console.log("1");
        if (params["cameraId"]) {
          return this.cameraService.aPIV1CameraControllerCameraCameraIdGet(params["cameraId"], this.storgeService.getAccessToken());
        } else {
          return of(null);
        }
      }),
      filter(value => value !== null && value !== undefined),
      distinctUntilChanged(),
      switchMap(camera => {
        console.log("2");
        return this.streamingService.aPIV1StreamingControllerStreamIdCameraIdGet(camera.cameraId!, this.storgeService.getAccessToken()).pipe(
          tap((c) => { console.log("3: "); console.log(c); }),
          map(streamId => [camera, streamId] as [CameraDTO, string]),
        );
      }),
      takeUntil(this.destroy$),
    ).subscribe(cameraAndStreamId => this.initializeCamera(cameraAndStreamId[0], cameraAndStreamId[1]));
  }

  initializeCamera(camera: CameraDTO, streamId: string): void {
    console.log("start " + camera.cameraId);
    const apiURL: string = this.configurationService.getAPIURL();
    if (this.player != null) {
      this.player.dispose();
    }
    this.player = videojs('videoPlayer', {
      techOrder: ['html5'],
      sources: [{
        src: `https://${apiURL}/API/v1/StreamingController/Stream/${streamId}`,
        type: 'application/dash+xml'
      }]
    });
  }
}
