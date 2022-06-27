import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { CartItem } from '../models/cart.model';
import {
  OrderItem, OrderCreateRequest,
  OrderOperationResponse,
  OrdersResult
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

  getUserOrders(email: string): Observable<OrdersResult> {
    const userEmail = email.length > 0
      ? email : this.authService.getUserEmail();
    if (!userEmail)
      return of<OrdersResult>({
        isSuccess: false,
        status: 'not_signed_in',
        message: "Not signed in"
      });
    return this.http.get<OrdersResult>(
      this.userEndpoint + `/all?email=${userEmail}`, this.httpOptions);
  }

  getAdminOrders(email: string): Observable<OrdersResult> {
    const userEmail = email.length > 0
      ? email : this.authService.getUserEmail();
    if (!userEmail)
      return of<OrdersResult>({
        isSuccess: false,
        status: 'not_signed_in',
        message: "Not signed in"
      });
    return this.http.get<OrdersResult>(
      this.userEndpoint + `/admin`, this.httpOptions);
  }

  getOrder(orderId: number): Observable<OrderOperationResponse> {
    if (!this.authService.isLoggedIn())
      return of<OrderOperationResponse>({
        isSuccess: false,
        status: 'not_signed_in',
        message: "Not signed in"
      });
    return this.http.get<OrderOperationResponse>(
      this.userEndpoint + `?orderId=${orderId}`, this.httpOptions);
  }

  setOrderStatus(orderId: number, newStatus: string) {
    if (!this.authService.isLoggedIn())
      return of<OrderOperationResponse>({
        isSuccess: false,
        status: 'not_signed_in',
        message: "Not signed in"
      });
    return this.http.post<OrderOperationResponse>(
      this.userEndpoint + `/status?orderId=${orderId}&newStatus=${newStatus}`,
        this.httpOptions);
  }
}
