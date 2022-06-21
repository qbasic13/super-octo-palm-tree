import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CatalogService {
  private endpoint = "/api/books";
  constructor(private http: HttpClient) { }

  getCatalogPage(page: number, items: number): Observable<any> {
    return this.http.get(this.endpoint + `?page=${page}&items=${items}`);
  }
}
