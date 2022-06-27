import { ChangeDetectorRef, Component } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { Order } from '../models/order.model';
import { AuthService } from '../services/auth.service';
import { OrderService } from '../services/order.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { MatDialog } from '@angular/material/dialog';
import { OrderDetailsComponent } from '../order-details/order-details.component';

@Component({
  selector: 'app-admin-orders',
  templateUrl: './admin-orders.component.html',
  styleUrls: ['./admin-orders.component.css']
})
export class AdminOrdersComponent {
  ordersList: Order[];
  noOrders: boolean = true;
  isLoading: boolean = true;
  displayedColumns: string[] = ['id', 'email', 'phone', 'name', 'price', 'createdAt', 'status', 'actions'];
  dataSource: any;

  constructor(
    private authService: AuthService,
    private orderService: OrderService,
    private snack: SnackNotifyComponent,
    private router: Router,
    public orderDetailsDialog: MatDialog,
    private changeDetectorRefs: ChangeDetectorRef
  ) {

    this.ordersList = [];
    this.isLoading = true;
    if (this.hasRole('admin')) {
      this.orderService.getAdminOrders('').subscribe(
        res => {
          if (res.isSuccess) {
            this.ordersList = res.orders!;
            this.dataSource = new MatTableDataSource(this.ordersList);
            this.noOrders = this.ordersList!.length == 0;
            this.isLoading = false;
          } else {
            this.ordersList = [];
            this.dataSource = new MatTableDataSource(this.ordersList);
            snack.openSnackBar(res.message ?? res.status, 'Ok');
            this.noOrders = true;
            this.isLoading = false;
          }
          changeDetectorRefs.detectChanges();
        },
        error => {
          snack.openSnackBar('Unknown error occured', 'Ok');
          this.noOrders = true;
          this.isLoading = false;
        }
      )
    } else {
      this.isLoading = false;
    }
  }

  getPrice(price?: number) {
    return !price ? 0 : price.toFixed(2);
  }

  startOrderDelivery(id: number) {
    this.orderService.setOrderStatus(id, 'being_delivered').subscribe(
      orderOpRes => {
        if (orderOpRes.isSuccess) {
          this.snack.openSnackBar(orderOpRes.message ?? orderOpRes.status, 'Ok');
          this.forceReload();
        } else {
          this.snack.openSnackBar(orderOpRes.message ?? orderOpRes.status, 'Ok');
        }
      },
      error => {
        this.snack.openSnackBar('Unknown error occured', 'Ok');
      }
    );
  }

  getOrderDetails(itemId: number) {
    this.orderService.getOrder(itemId).subscribe(
      orderOpRes => {
        if (orderOpRes.isSuccess) {
          this.orderDetailsDialog.open(OrderDetailsComponent, {
            data: {
              order: orderOpRes.order!
            },
            minWidth: '80vw',
            maxHeight: '95vh'
          });
        } else {
          this.snack.openSnackBar(orderOpRes.message ?? orderOpRes.status, 'Ok');
        }
      },
      error => {
        this.snack.openSnackBar('Unknown error occured', 'Ok');
      }
    );
  }

  isSignedIn() {
    return this.authService.isLoggedIn();
  }

  hasRole(...params: string[]) {
    return this.authService.hasRole(...params);
  }

  forceReload() {
    let currentUrl = this.router.url;
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate([currentUrl]);
      console.log(currentUrl);
    });
  }
}
