import { Injectable } from '@angular/core';
import { Cart, CartDetailsResponse, CartItem } from '../models/cart.model';
import { AuthService } from './auth.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BookService } from './book.service';
import { BookDetails } from '../models/books.model';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  constructor(
    private authService: AuthService,
    private bookService: BookService
  ) { }

  loadCartData(): Cart {
    const cart = localStorage.getItem('cart');
    if (!cart) {
      const cart: Cart = { items:[]};
      return cart;
    }
    return JSON.parse(cart);
  }

  saveCartData(cart: Cart) {
    localStorage.setItem('cart', JSON.stringify(cart));
  }

  clearCartData() {
    localStorage.removeItem('cart');
    this.loadCartData();
  }

  getCartUniqueItemsCount(): number {
    const cart = this.loadCartData();
    return cart.items.length;
  }

  addToCart(cart: Cart, isbn: string): Cart {
    if (!cart.items.find(x => x.isbn == isbn)) {
      const newItem: CartItem = {
        isbn: isbn,
        desiredQuantity: 1
      };
      cart.items.push(newItem);
    }
    return cart;
  }

  isInCart(isbn: string): boolean {
    const cart = this.loadCartData();
    return cart.items.findIndex(x => x.isbn == isbn) >= 0;
  }

  getFullCartData(cart: Cart): Observable<CartDetailsResponse> {
    let isbns: string[] = [];
    for (var bookItem of cart.items) {
      isbns.push(bookItem.isbn);
    }
    return this.bookService.getCartDetails(isbns);
  }

  getCartTotal(fullCart: Cart): number {
    let i: number = 0;
    let total = 0;
    for (i = 0; i < fullCart.items.length; i++) {
      total += fullCart.items[i].details?.price!;
    }
    return total;
  }

  removeFromCart(cart: Cart, isbn: string): Cart {
    const itemIndx = cart.items.findIndex(x => x.isbn == isbn);
    if (itemIndx >= 0) {
      cart.items.splice(itemIndx, 1);
    }
    return cart;
  }
}
