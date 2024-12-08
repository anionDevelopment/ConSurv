import { Component, OnInit, inject } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CameraDTO, CameraService, UpdateCameraDTO } from '../../../generated/con-surv-backend';
import { FormControl, FormGroup } from '@angular/forms';
import { StorageService } from '../../../services/storage.service';
import { Observable, switchMap } from 'rxjs';

@Component({
  selector: 'app-edit-camera-dialog',
  standalone: false,
  templateUrl: './edit-camera-dialog.component.html',
  styleUrl: './edit-camera-dialog.component.scss'
})
export class EditCameraDialogComponent {

  private cameraData: CameraDTO;
  form: FormGroup = new FormGroup({
    name: new FormControl(''),
    rtspstreamurl: new FormControl(''),
    isonvifcamera: new FormControl(''),
    recordmode: new FormControl(''),
  });

  constructor(private dialogRef: MatDialogRef<EditCameraDialogComponent>, private cameraService: CameraService, private storageService: StorageService) {
    const data: any = inject<CameraDTO>(MAT_DIALOG_DATA);
    this.cameraData = data.camera;
    this.form.get('name')!.setValue(this.cameraData.name);
    this.form.get('rtspstreamurl')!.setValue(this.cameraData.videoInformation!.streamURL);
    this.form.get('isonvifcamera')!.setValue(this.cameraData.videoInformation!.isONVIFCamera);
    console.log("c");
    console.log(this.cameraData.recordMode!.recordMode);
    this.form.get('recordmode')!.setValue(this.cameraData.recordMode!.recordMode);
  }

  save() {
    const updated: UpdateCameraDTO = {
      cameraId: this.cameraData.cameraId,
      name: this.form.get('name')!.value,
      recordMode: {
        recordMode: this.form.get('recordmode')!.value,
      },
      videoInformation: {
        streamURL: this.form.get('rtspstreamurl')!.value,
        isONVIFCamera: this.form.get('isonvifcamera')!.value,
      },
    };
    this.cameraService.aPIV1CameraControllerUpdateCameraPut(this.storageService.getAccessToken(), updated).pipe(
      switchMap(() => {
        const result: Observable<CameraDTO> = this.cameraService.aPIV1CameraControllerCameraCameraIdGet(updated.cameraId!, this.storageService.getAccessToken());
        return result;
      })
    ).subscribe((cameraDTO: CameraDTO) => {
      console.log("x");
      console.log(cameraDTO);
      this.dialogRef.close(cameraDTO);
    });
  }

  cancel() {
    this.dialogRef.close();
  }

}
