import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from './angular-material.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { AuthInterceptor } from 'src/app/helpers/auth.interceptor';

import { AppComponent } from './app.component';
import { CatalogComponent } from './catalog/catalog.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { FooterComponent } from './footer/footer.component';
import { BookSearchBarComponent } from './book-search-bar/book-search-bar.component';
import { NavMenuIconsComponent } from './nav-menu-icons/nav-menu-icons.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { SnackNotifyComponent } from './snack-notify/snack-notify.component';
import { VerifyEmailComponent } from './verify-email/verify-email.component';
import { BookDetailsComponent } from './book-details/book-details.component';
import { EditBookComponent } from './edit-book/edit-book.component';
import { BookCoverUploadComponent } from './book-cover-upload/book-cover-upload.component';
import { CartComponent } from './cart/cart.component';
import { OrdersComponent } from './orders/orders.component';
import { OrderDetailsComponent } from './order-details/order-details.component';
import { AdminOrdersComponent } from './admin-orders/admin-orders.component';
import { ProfileComponent } from './profile/profile.component';

@NgModule({
  declarations: [
    AppComponent,
    CatalogComponent,
    NavMenuComponent,
    FooterComponent,
    BookSearchBarComponent,
    NavMenuIconsComponent,
    SignInComponent,
    SignUpComponent,
    SnackNotifyComponent,
    VerifyEmailComponent,
    BookDetailsComponent,
    EditBookComponent,
    BookCoverUploadComponent,
    CartComponent,
    OrdersComponent,
    OrderDetailsComponent,
    AdminOrdersComponent,
    ProfileComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule, 
    BrowserAnimationsModule, 
    AppRoutingModule, 
    BrowserAnimationsModule,
    AngularMaterialModule,
    NgxPaginationModule
  ],
  providers: [SnackNotifyComponent,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
