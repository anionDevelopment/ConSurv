import { Component, Input, OnInit } from '@angular/core';
import { CameraService } from '../../../generated/con-surv-backend';
import { StorageService } from '../../../services/storage.service';

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
      console.log("if");
      this.cameraService.aPIV1CameraControllerGetPreviewCameraIdMaximalHeightMaximalWidthGet(this.cameraId, 75, 100, this.storageService.getAccessToken()).subscribe((result) => {
        this.image = 'data:image/png;base64,' + result;//TODO update this picture every 5 seconds
        console.log("set");
      });
    } else {
      console.log("else");
      //TODO throw error
    }
  }
}
