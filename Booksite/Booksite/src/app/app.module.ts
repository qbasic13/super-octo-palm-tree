import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from './angular-material.module';
import { NgxPaginationModule } from 'ngx-pagination';

import { AppComponent } from './app.component';
import { CatalogComponent } from './catalog/catalog.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { FooterComponent } from './footer/footer.component';
import { BookSearchBarComponent } from './book-search-bar/book-search-bar.component';

@NgModule({
  declarations: [
    AppComponent,
    CatalogComponent,
    NavMenuComponent,
    FooterComponent,
    BookSearchBarComponent
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
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
