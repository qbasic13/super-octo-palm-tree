import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminOrdersComponent } from './admin-orders/admin-orders.component';
import { BookDetailsComponent } from './book-details/book-details.component';
import { CartComponent } from './cart/cart.component';
import { CatalogComponent } from './catalog/catalog.component';
import { EditBookComponent } from './edit-book/edit-book.component';
import { OrdersComponent } from './orders/orders.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { VerifyEmailComponent } from './verify-email/verify-email.component';

const routes: Routes = [
  { path: '', component: CatalogComponent, pathMatch: 'full' },
  { path: 'signIn', component: SignInComponent },
  { path: 'signUp', component: SignUpComponent },
  { path: 'verify', component: VerifyEmailComponent },
  { path: 'book/:isbn', component: BookDetailsComponent },
  { path: 'cart', component: CartComponent },
  { path: 'orders', component: OrdersComponent },
  { path: 'orders/admin', component: AdminOrdersComponent }
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
