import { Component } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { CameraDTO, CameraService, RecordModeDTO } from '../../../generated/con-surv-backend';
import { StorageService } from '../../../services/storage.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { EditCameraDialogComponent } from '../edit-camera-dialog/edit-camera-dialog.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-cameras-list',
  standalone: false,
  templateUrl: './cameras-list.component.html',
  styleUrl: './cameras-list.component.scss'
})
export class CamerasListComponent {

  userIdModerator: boolean | null = null;
  cameras: CameraDTO[] = [];
  displayedColumns: string[] = ["name", "mode", "state"];
  recordMode$: Observable<RecordModeDTO> | null = null;
  constructor(userDataService: UserDataService, private cameraService: CameraService, private storageService: StorageService, private router: Router, private matDialog: MatDialog) {
    userDataService.userIsModerator().subscribe((isModerator) => {
      this.userIdModerator = isModerator;
      const newColumnsList = [...this.displayedColumns];
      newColumnsList.push("options");
      this.displayedColumns = newColumnsList;
    });
    this.cameraService.aPIV1CameraControllerCamerasGet(this.storageService.getAccessToken()).subscribe((cameras => {
      this.cameras = cameras;
    }));
  }

  addCamera() {
    this.cameraService.aPIV1CameraControllerCreateCameraPost(this.storageService.getAccessToken()).subscribe((cameraId) => {
      this.cameraService.aPIV1CameraControllerCameraCameraIdGet(cameraId, this.storageService.getAccessToken()).subscribe((camera) => {
        const newCameraList = [...this.cameras];
        newCameraList.push(camera)
        this.cameras = newCameraList;
      });
    });
  }

  removeCamera(camera: CameraDTO) {
    throw new Error('Method not implemented.');
  }

  editCamera(camera: CameraDTO) {
    const dialogRef = this.matDialog.open(EditCameraDialogComponent, {
      data: {
        camera: camera,
      },
    });
    dialogRef.afterClosed().subscribe((updatedCamera: CameraDTO) => {
      var newList: CameraDTO[] = [...this.cameras];
      this.cameras = newList;
    });
  }

  onCameraClick(camera: CameraDTO) {
    this.router.navigate(["user", "camera"], { queryParams: { cameraId: camera.cameraId } });
  }

}
