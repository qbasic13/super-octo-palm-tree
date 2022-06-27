import { ChangeDetectorRef, Component } from '@angular/core';
import { Cart, CartItem } from '../models/cart.model';
import { AuthService } from '../services/auth.service';
import { CartService } from '../services/cart.service';
import { DataSource } from '@angular/cdk/collections';
import { BookDetails } from '../models/books.model';
import { MatTableDataSource } from '@angular/material/table';
import { OrderService } from '../services/order.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { OrderOperationResponse } from '../models/order.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent {
  cart: Cart;
  noItemsInCart: boolean = true;
  isLoading: boolean = true;
  displayedColumns: string[] = ['isbn', 'title', 'author', 'price', 'quantity', 'actions'];
  dataSource: any;
  constructor(
    private authService: AuthService,
    private cartService: CartService,
    private orderService: OrderService,
    private snack: SnackNotifyComponent,
    private router: Router,
    private changeDetectorRefs: ChangeDetectorRef
  ) {
    this.isLoading = true;
    this.cart = this.cartService.loadCartData();
    if (this.cart.items.length > 0) {
      this.cartService.getFullCartData(this.cart).subscribe(
        cartDetailsResponse => {
          for (var bookDetail of cartDetailsResponse.details) {
            let idx = this.cart.items.findIndex
              (ci => ci.isbn === bookDetail.isbn);
            this.cart.items[idx].details = bookDetail;
          }
          this.dataSource = new MatTableDataSource(this.cart.items);
          this.noItemsInCart = false;
          this.isLoading = false;
        }
      );
    } else {
      this.noItemsInCart = true;
      this.isLoading = false;
    }
  }

  getCartTotal() {
    return this.cartService.getCartTotal(this.cart).toFixed(2);
  }

  removeBookFromCart(isbn: string) {
    this.cart = this.cartService.removeFromCart(this.cart, isbn);
    this.cartService.saveCartData(this.cart);
    this.dataSource = new MatTableDataSource(this.cart.items);
    if (this.cartService.getCartUniqueItemsCount() == 0) {
      this.noItemsInCart = true;
    }
    this.changeDetectorRefs.detectChanges();
  }

  createOrder() {
    this.isLoading = true;
    this.orderService.createOrder(this.cart.items).subscribe(
      orderOpResponse => {
        const orderRes = orderOpResponse as OrderOperationResponse;
        if (!orderRes.isSuccess) {
          this.snack.openSnackBar(orderRes.message ?? orderRes.status, 'Ok');
        } else {
          this.cartService.clearCartData();
          this.snack.openSnackBar(orderRes.message ?? orderRes.status, 'Ok');
          this.router.navigateByUrl('/order/' + orderRes.order!.id);
        }
        this.isLoading = false;
      },
      error => {
        this.snack.openSnackBar(error, 'Ok');
        this.isLoading = false;
      });
  }

  isSignedIn() {
    return this.authService.isLoggedIn();
  }

  hasRole(...params: string[]) {
    return this.authService.hasRole(...params);
  }
}
