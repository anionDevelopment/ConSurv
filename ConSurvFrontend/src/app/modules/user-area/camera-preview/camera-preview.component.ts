import { Component, Input, OnInit } from '@angular/core';
import { CameraService } from '../../../generated/con-surv-backend';
import { StorageService } from '../../../services/storage.service';
import { interval, startWith, switchMap } from 'rxjs';

@Component({
  selector: 'app-camera-preview',
  standalone: false,
  templateUrl: './camera-preview.component.html',
  styleUrl: './camera-preview.component.scss'
})
export class CameraPreviewComponent implements OnInit {

  @Input()
  cameraId: string | null = null;
  image: string = "";
  constructor(private cameraService: CameraService, private storageService: StorageService) {
  }

  ngOnInit(): void {
    if (this.cameraId) {
      interval(5000).pipe( 
        startWith(0),
        switchMap(() => this.cameraService.aPIV1CameraControllerGetPreviewCameraIdGet(this.cameraId!, this.storageService.getAccessToken())
        )).subscribe((result) => {
          this.image = 'data:image/png;base64,' + result;
        });
    } else {
      //TODO throw error
    }
  }
}
