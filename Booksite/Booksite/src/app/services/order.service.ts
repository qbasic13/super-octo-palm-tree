import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CartItem } from '../models/cart.model';
import {
  OrderItem, OrderCreateRequest,
  OrderOperationResponse
} from '../models/order.model';
import { AuthService } from './auth.service';
import { CartService } from './cart.service';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  userEndpoint = 'api/order';
  adminEndpoint = 'api/order/admin';

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  };

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private cartService: CartService
  ) { }

  createOrder(cartItems: CartItem[]): Observable<OrderOperationResponse> {
    let orderRequest: OrderCreateRequest = { orderItems: [] };
    for (var cartItem of cartItems) {
      const orderItem: OrderItem = {
        isbn: cartItem.isbn,
        quantity: cartItem.desiredQuantity
      };
      orderRequest.orderItems.push(orderItem);
    }
    return this.http.post<OrderOperationResponse>(
      this.userEndpoint + '/create', orderRequest, this.httpOptions);
  }
}
