import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { CameraService } from '../../../generated/con-surv-backend';
import { StorageService } from '../../../services/storage.service';
import { interval, startWith, Subject, switchMap, takeUntil } from 'rxjs';

@Component({
  selector: 'app-camera-preview',
  standalone: false,
  templateUrl: './camera-preview.component.html',
  styleUrl: './camera-preview.component.scss'
})
export class CameraPreviewComponent implements OnInit, OnDestroy {

  @Input()
  cameraId: string | null = null;
  image: string = "";
  private destroy$ = new Subject<void>();
  constructor(private cameraService: CameraService, private storageService: StorageService) {
  }

  ngOnInit(): void {
    if (this.cameraId) {
      interval(5000).pipe(
        startWith(0),
        switchMap(() => this.cameraService.aPIV3CameraControllerGetPreviewCameraIdGet(this.cameraId!, this.storageService.getAccessToken())
        ),
        takeUntil(this.destroy$),
      ).subscribe((result) => {
        this.image = 'data:image/png;base64,' + result;
      });
    }
  }

  ngOnDestroy(): void {
    //without this the interval would keep requesting previews for a camera which is not displayed anymore
    this.destroy$.next();
    this.destroy$.complete();
  }

  popupVisible = false;
  popupX = 0;
  popupY = 0;

  updatePopupPosition(event: MouseEvent) {
    this.popupVisible = true;
    const offset = 10;
    this.popupX = event.clientX + offset;
    this.popupY = event.clientY + offset;
    const maxX = window.innerWidth - 420;
    const maxY = window.innerHeight - 420;
    if (this.popupX > maxX) this.popupX = maxX;
    if (this.popupY > maxY) this.popupY = maxY;
  }
}
