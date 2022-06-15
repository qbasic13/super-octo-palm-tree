import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CatalogComponent } from './catalog/catalog.component';

const routes: Routes = [
  { path: '', component: CatalogComponent, pathMatch: 'full' },
  { path: 'catalog', component: CatalogComponent }
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
