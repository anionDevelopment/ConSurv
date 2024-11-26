import { Component, OnInit, inject } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CameraDTO, CameraService } from '../../../generated/con-surv-backend';
import { FormControl, FormGroup } from '@angular/forms';
import { StorageService } from '../../../services/storage.service';

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
  });
  constructor(private dialogRef: MatDialogRef<EditCameraDialogComponent>, private cameraService: CameraService, private storageService: StorageService) {
    const data: any = inject<CameraDTO>(MAT_DIALOG_DATA);
    this.cameraData = data.camera;
    this.form.get('name')!.setValue(this.cameraData.name);
    //this.form.get('rtspstreamurl')!.setValue(this.cameraData.videoType!.streamURL || '');
  }
  save() {
    this.cameraService.aPIV1CameraControllerUpdateCameraPost(this.storageService, {
      cameraId: this.cameraData.cameraId,
      name: this.form.get('name')!.value,
      recordMode: {
        //TODO set values from dialog
      },
      videoType: {
        //TODO set values from dialog
      },
    });
    this.dialogRef.close();
  }
  cancel() {
    this.dialogRef.close();
  }

}
