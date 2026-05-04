import { Component, inject } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, FormGroup } from '@angular/forms';
import { StorageService } from '../../../services/storage.service';
import { BehaviorSubject, Observable, Subject, switchMap } from 'rxjs';
import { CameraDTO, CameraService, RecordModeDTO, UpdateCameraDTO } from '../../../generated/con-surv-backend';

@Component({
  selector: 'app-edit-camera-dialog',
  standalone: false,
  templateUrl: './edit-camera-dialog.component.html',
  styleUrl: './edit-camera-dialog.component.scss'
})
export class EditCameraDialogComponent {

  private cameraData: CameraDTO;
  form: FormGroup;
  nameFormControl: FormControl<string>;
  rtspstreamurlFormControl: FormControl<string>;
  supportsPTZViaONVIFFormControl: FormControl<boolean>;
  recordmodeFormControl: FormControl<RecordModeDTO>;
  recordmodeSelectFormControl: FormControl<string>;
  recordmode$: BehaviorSubject<RecordModeDTO>;

  constructor(private dialogRef: MatDialogRef<EditCameraDialogComponent>, private cameraService: CameraService, private storageService: StorageService) {
    const data: any = inject<CameraDTO>(MAT_DIALOG_DATA);
    this.cameraData = data.camera;
    this.nameFormControl = new FormControl<any>(this.cameraData.name!);
    this.rtspstreamurlFormControl = new FormControl<any>(this.cameraData.videoInformationDTO!.streamURL!);
    this.supportsPTZViaONVIFFormControl = new FormControl<any>(this.cameraData.videoInformationDTO!.supportsPTZViaONVIF!);
    this.recordmodeFormControl = new FormControl<any>(this.cameraData.recordModeDTO!);
    this.recordmodeSelectFormControl = new FormControl<any>(this.cameraData.recordModeDTO!.recordMode);
    this.form = new FormGroup({
      name: this.nameFormControl,
      rtspstreamurl: this.rtspstreamurlFormControl,
      supportsPTZViaONVIF: this.supportsPTZViaONVIFFormControl,
      recordmode: this.recordmodeFormControl,
      recordmodeselect: this.recordmodeSelectFormControl,
    });
    this.recordmode$ = new BehaviorSubject<RecordModeDTO>(this.cameraData.recordModeDTO!);
    this.recordmodeSelectFormControl.valueChanges.subscribe((newRecordMode: string) => this.recordmodeFormControl.setValue({
      recordMode: newRecordMode
    }));
    this.recordmodeFormControl.valueChanges.subscribe((newValue: RecordModeDTO) => this.recordmode$.next(newValue));
  }

  save() {
    const updated: UpdateCameraDTO = {
      cameraId: this.cameraData.cameraId,
      name: this.form.get('name')!.value,
      recordModeDTO: this.form.get('recordmode')!.value,
      videoInformationDTO: {
        streamURL: this.form.get('rtspstreamurl')!.value,
        supportsPTZViaONVIF: this.form.get('supportsPTZViaONVIF')!.value,
      },
    };
    this.cameraService.aPIV3CameraControllerUpdateCameraPut(this.storageService.getAccessToken(), updated).pipe(
      switchMap(() => {
        const result: Observable<CameraDTO> = this.cameraService.aPIV3CameraControllerCameraCameraIdGet(updated.cameraId!, this.storageService.getAccessToken());
        return result;
      })
    ).subscribe((cameraDTO: CameraDTO) => {
      this.dialogRef.close(cameraDTO);
    });
  }

  cancel() {
    this.dialogRef.close();
  }

}
