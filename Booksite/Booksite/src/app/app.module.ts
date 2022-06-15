import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list'; 

import { AppComponent } from './app.component';
import { CatalogComponent } from './catalog/catalog.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

@NgModule({
  declarations: [
    AppComponent,
    CatalogComponent,
    NavMenuComponent,
  ],
  imports: [
    BrowserModule, 
    HttpClientModule, 
    BrowserAnimationsModule, 
    AppRoutingModule, 
    BrowserAnimationsModule,
    MatButtonModule, 
    MatIconModule, 
    MatToolbarModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatGridListModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
