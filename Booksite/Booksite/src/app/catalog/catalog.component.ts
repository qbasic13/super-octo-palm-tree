import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';


@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.css']
})
export class CatalogComponent implements OnInit {
  public books?: Book[];
  constructor(http: HttpClient) { 
    http.get<Book[]>(environment.baseUrl+'api/books').subscribe(
      result => { this.books = result; },
      error => console.error(error)
    );
  }

  ngOnInit(): void {
  }
}

interface Book {
  isbn: string;
  title: string;
  author: string;
  quantity: number;
  price: number;
  coverFile: string;
}
