import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Input,  } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable, from } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { fromFetch } from 'rxjs/fetch';
import { CatalogService } from '../catalog.service';

interface Book {
  isbn: string;
  title: string;
  author: string;
  quantity: number;
  price: number;
  coverFile: string;
}

interface CatalogPage {
  books: Book[],
  count: number
}

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
  constructor(private catalogService: CatalogService) { };

  ngOnInit(): void {
    this.getPage(this.currPage);
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
