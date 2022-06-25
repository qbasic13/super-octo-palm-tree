import { HttpClient } from '@angular/common/http';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CatalogService } from 'src/app/services/catalog.service';
import { Book, CatalogPage } from 'src/app/models/books.model';
import { EditBookComponent } from '../edit-book/edit-book.component';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.css']
})

export class CatalogComponent {
  books: any;
  pageItems = 5;
  currPage: number = 1;
  count: number = 0;
  loading: boolean = true;
  constructor(
    private catalogService: CatalogService,
    private authService: AuthService,
    public addBookDialog: MatDialog) { };

  ngOnInit(): void {
    this.getPage(this.currPage);
  }

  openAddBookDialog() {
    this.addBookDialog.open(EditBookComponent, {
      data: {
        isAdding: true
      },
      maxHeight: '80vh'
    });
  }

  hasRole(...params: string[]) {
    return this.authService.hasRole(...params);
  }

  getPage(page: number): void {
    this.loading = true;
    this.catalogService.getCatalogPage(page, this.pageItems)
      .subscribe(
        (response) => {
          this.books = (response as CatalogPage).books;
          this.count = (response as CatalogPage).count;
          this.loading = false;
          this.currPage = page;
        },
        (error) => {
          console.log(error);
        }
      );
  }
}
