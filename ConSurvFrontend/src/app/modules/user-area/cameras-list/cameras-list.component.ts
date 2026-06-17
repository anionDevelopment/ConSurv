import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { StorageService } from '../../../services/storage.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { EditCameraDialogComponent } from '../edit-camera-dialog/edit-camera-dialog.component';
import { BehaviorSubject } from 'rxjs';
import { CameraDTO, CameraService, RecordModeDTO } from '../../../generated/con-surv-backend';

@Component({
  selector: 'app-cameras-list',
  standalone: false,
  templateUrl: './cameras-list.component.html',
  styleUrl: './cameras-list.component.scss'
})
export class CamerasListComponent implements OnInit {

  userIdModerator: boolean | null = null;
  cameras: CameraDTO[] = [];
  displayedColumns: string[] = ["preview", "name", "mode", "state"];
  recordModes: Map<string, BehaviorSubject<RecordModeDTO>> = new Map<string, BehaviorSubject<RecordModeDTO>>();
  constructor(private userDataService: UserDataService, private cameraService: CameraService, private storageService: StorageService, private router: Router, private matDialog: MatDialog) {
  }
  ngOnInit(): void {
    this.userDataService.userIsModerator().subscribe((isModerator) => {
      this.userIdModerator = isModerator;
      const newColumnsList = [...this.displayedColumns];
      newColumnsList.push("options");
      this.displayedColumns = newColumnsList;
    });
    this.cameraService.aPIV3CameraControllerCamerasGet(this.storageService.getAccessToken()).subscribe(((cameras: CameraDTO[]) => {
      cameras.forEach(camera => {
        this.recordModes.set(camera.cameraId!, new BehaviorSubject<RecordModeDTO>(camera.recordModeDTO!))
      });
      this.cameras = cameras;
    }));
  }

  addCamera() {
    this.cameraService.aPIV3CameraControllerCreateCameraPost(this.storageService.getAccessToken()).subscribe((cameraId: string) => {
      this.cameraService.aPIV3CameraControllerCameraCameraIdGet(cameraId, this.storageService.getAccessToken()).subscribe((camera: CameraDTO) => {
        const newCameraList = [...this.cameras];
        newCameraList.push(camera)
        this.cameras = newCameraList;
        this.recordModes.set(camera.cameraId!, new BehaviorSubject<RecordModeDTO>(camera.recordModeDTO!))
      });
    });
  }

  removeCamera(camera: CameraDTO) {
    this.cameraService.aPIV3CameraControllerRemoveCameraCameraIdDelete(camera.cameraId!, this.storageService.getAccessToken()).subscribe(() => {
      const newList: CameraDTO[] = [];
      this.cameras.forEach(existingCamera => {
        if (existingCamera.cameraId !== camera.cameraId) {
          newList.push(existingCamera);
        }
      });
      this.cameras = newList;
    });
  }

  editCamera(camera: CameraDTO) {
    const dialogRef = this.matDialog.open(EditCameraDialogComponent, {
      data: {
        camera: camera,
      },
    });
    dialogRef.afterClosed().subscribe((updatedCamera: CameraDTO) => {
      if (updatedCamera !== undefined) {//updatedCamera is undefined if the dialog was canceled
        const newList: CameraDTO[] = [];
        this.cameras.forEach(camera => {
          if (camera.cameraId === updatedCamera.cameraId) {
            newList.push(updatedCamera);
            this.recordModes.get(updatedCamera.cameraId!)?.next(updatedCamera.recordModeDTO!);
          } else {
            newList.push(camera);
          }
        });
        this.cameras = newList;
      }
    });
  }

  onCameraClick(camera: CameraDTO) {
    this.router.navigate(["user", "camera"], { queryParams: { cameraId: camera.cameraId } });
  }
}
