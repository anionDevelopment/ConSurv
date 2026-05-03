import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CameraDTO, CameraService, StreamingService } from '../../../generated/con-surv-backend';
import { distinctUntilChanged, filter, of, Subject, switchMap, takeUntil, tap } from 'rxjs';
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
export class CameraComponent implements OnInit, OnDestroy {

  player: Player | null = null;
  camera: CameraDTO | null = null;
  private destroy$ = new Subject<void>();
  options: any = {}
  information: string = "";
  constructor(private activatedRoute: ActivatedRoute, private cameraService: CameraService, private storgeService: StorageService, private streamingService: StreamingService, private configurationService: ConfigurationService) {
  }

  ngOnDestroy(): void {
    if (this.player != null) {
      this.player.dispose();
    }
  }

  ngOnInit(): void {
    const accessToken: string = this.storgeService.getAccessToken();
    (videojs as any).Vhs.xhr.beforeRequest = function (options: any) {
      options.headers = options.headers || {};
      options.headers['X-AccessToken'] = accessToken;
      return options;
    };
    this.activatedRoute.queryParams.pipe(
      switchMap(params => {
        if (params["cameraId"]) {
          return this.cameraService.aPIV3CameraControllerCameraCameraIdGet(params["cameraId"], this.storgeService.getAccessToken());
        } else {
          return of(null);
        }
      }),
      filter(value => value !== null && value !== undefined),
      distinctUntilChanged(),
      takeUntil(this.destroy$),
    ).subscribe(camera => this.initializeCamera(camera));
  }

  initializeCamera(camera: CameraDTO): void {
    this.camera = camera;
    const apiURL: string = this.configurationService.getAPIURL();
    this.information = `Camera ${camera.name} (Id: ${camera.cameraId})`;
    const majorVersion = 3;//TODO retrieve this from package.json
    const url = `${apiURL}/API/v${majorVersion}/StreamingController/Stream/${camera.cameraId}/stream.m3u8`;
    this.player = videojs("videoPlayer", {
      autoplay: true,
      controls: false,
      sources: [
        {
          src: url,
          type: 'application/x-mpegURL'
        }
      ],
    }, function onPlayerReady() {
      //nothing to do
    });
  }

  sendONVIFCommand(commandType: string, direction: string): void {
    if (!this.camera?.cameraId) { return; }
    this.cameraService.aPIV3CameraControllerRunONVIFCommandCameraIdPost(
      this.camera.cameraId,
      { commandType, direction }
    ).pipe(takeUntil(this.destroy$)).subscribe();
  }
}
