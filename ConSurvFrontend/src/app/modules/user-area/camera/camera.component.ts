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

  player: Player | null = null;
  private destroy$ = new Subject<void>();
  options: any = {}
  constructor(private activatedRoute: ActivatedRoute, private cameraService: CameraService, private storgeService: StorageService, private streamingService: StreamingService, private configurationService: ConfigurationService) {
  }

  ngOnInit(): void {
    this.activatedRoute.queryParams.pipe(
      switchMap(params => {
        if (params["cameraId"]) {
          return this.cameraService.aPIV1CameraControllerCameraCameraIdGet(params["cameraId"], this.storgeService.getAccessToken());
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
    const apiURL: string = this.configurationService.getAPIURL();
    const url = `${apiURL}/API/v1/StreamingController/Stream/${camera.cameraId}/stream.m3u8`;
    if (this.player !== null) {
      this.player.dispose();
      this.player = null;
    }
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
      console.log('onPlayerReady', this);
    });
    /*
console.log("start stream " + camera.cameraId + " " + url);
this.options = {
  autoplay: true,
  controls: false,
  sources: [
    {
      src: url,
      type: 'application/x-mpegURL'
    }
  ],
};
*/
  }
}
