import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BookSearchService {
  private endpoint = "api/books/search";
  constructor(private http: HttpClient) { }

  search(title: string): Observable<any> {
    const trimmedTitle = title.trim().toLowerCase();

    if (trimmedTitle.length < 3) {
      return of([]);
    }

    return this.http.get(
      this.endpoint + `?title=${title}`);
  }
}
