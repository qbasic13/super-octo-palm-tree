import { Component } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { SnackNotifyComponent } from 'src/app/snack-notify/snack-notify.component';

@Component({
  selector: 'app-nav-menu-icons',
  templateUrl: './nav-menu-icons.component.html',
  styleUrls: ['./nav-menu-icons.component.css']
})
export class NavMenuIconsComponent {
  
  constructor(
    private authService: AuthService,
    private snack: SnackNotifyComponent
  ) { }

  async signOut() {
    await this.authService.signOut();
    this.snack.openSnackBar('Successfuly signed out', 'Ok');
  }

  hasRole(role: string) {
    return localStorage.getItem('role') === role;
  }
  isLoggedIn() {
    return localStorage.getItem('email') !== null;
  }

}
