import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { faPlus, faSignInAlt, faInfoCircle, faSignOutAlt, faHome, faUser } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;

  faPlus = faPlus;
  faSignInAlt = faSignInAlt;
  faInfoCircle = faInfoCircle;
  faSignOutAlt = faSignOutAlt;
  faHome = faHome;
  faUser = faUser;

  constructor(public auth: AuthService,
    private router: Router) { }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout(): boolean {
    if (this.auth.logout()) {
      this.router.navigate([""]);
    }
    return false;
  }
}
