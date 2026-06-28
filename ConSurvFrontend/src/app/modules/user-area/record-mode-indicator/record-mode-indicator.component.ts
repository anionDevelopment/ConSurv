import { Component, Input, OnInit } from '@angular/core';
import { RecordModeDTO } from '../../../generated/con-surv-backend';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-record-mode-indicator',
  standalone: false,
  templateUrl: './record-mode-indicator.component.html',
  styleUrl: './record-mode-indicator.component.scss'
})
export class RecordModeIndicatorComponent {
  @Input()
  recordMode$!: Observable<RecordModeDTO>;

}
