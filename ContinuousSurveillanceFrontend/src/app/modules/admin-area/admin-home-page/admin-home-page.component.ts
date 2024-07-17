import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-admin-home-page',
  templateUrl: './admin-home-page.component.html',
  styleUrls: ['./admin-home-page.component.scss']
})
export class AdminHomePageComponent {
  isCollapsed: boolean = true;
  fragment: BehaviorSubject<string> = new BehaviorSubject('');
  dashboardOptions: AdminDashboardOption[] = [
    new AdminDashboardOption('Home','','house'),
    new AdminDashboardOption('Cameras','cameras','videocam'),
    new AdminDashboardOption('Users','users','group'),
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
class AdminDashboardOption {
  constructor(public name: string, public fragment: string, public icon: string) {

  }
}
