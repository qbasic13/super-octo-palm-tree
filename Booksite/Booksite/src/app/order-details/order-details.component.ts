import { Component, Inject } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Order } from '../models/order.model';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.css']
})
export class OrderDetailsComponent {
  order: Order;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.order = this.data.order!;
  }
}
