import { Component } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { SnackNotifyComponent } from 'src/app/snack-notify/snack-notify.component';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-nav-menu-icons',
  templateUrl: './nav-menu-icons.component.html',
  styleUrls: ['./nav-menu-icons.component.css']
})
export class NavMenuIconsComponent {
  
  constructor(
    private authService: AuthService,
    private cartService: CartService,
    private snack: SnackNotifyComponent
  ) { }

  async signOut() {
    await this.authService.signOut();
    this.snack.openSnackBar('Successfuly signed out', 'Ok');
  }

  getCartItems(): number {
    return this.cartService.getCartUniqueItemsCount();
  }

  hasRole(...params: string[]) {
    return this.authService.hasRole(...params);
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }

}
