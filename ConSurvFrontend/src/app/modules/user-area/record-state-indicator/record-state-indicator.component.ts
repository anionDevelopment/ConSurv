import { Component, Input } from '@angular/core';
import { RecordStateDTO as RecordStateDTO } from '../../../generated/con-surv-backend';

@Component({
  selector: 'app-record-state-indicator',
  standalone: false,
  templateUrl: './record-state-indicator.component.html',
  styleUrl: './record-state-indicator.component.scss'
})
export class RecordStateIndicatorComponent {
  @Input()
  recordState: RecordStateDTO | null = null;

}
