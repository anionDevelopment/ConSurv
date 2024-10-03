import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-user-home-page',
  templateUrl: './user-home-page.component.html',
  styleUrls: ['./user-home-page.component.scss']
})
export class UserHomePageComponent {
  isCollapsed: boolean = true;
  fragment: BehaviorSubject<string> = new BehaviorSubject('');
  dashboardOptions: UserDashboardOption[] = [
    new UserDashboardOption('Home', '', 'house'),
    new UserDashboardOption('Camera', 'camera', 'videocam'),
    new UserDashboardOption('Cameras', 'cameras', 'videocam'),
  ];
  constructor(
    private route: ActivatedRoute,
  ) {
    this.route.fragment.subscribe((fragmentOrNull: string | null) => {
      this.fragment.next(fragmentOrNull == null ? '' : fragmentOrNull!);
    });

  }

  toggleMenu() {
    this.isCollapsed = !this.isCollapsed;
  }
}
class UserDashboardOption {
  constructor(public name: string, public fragment: string, public icon: string) {

  }
}
