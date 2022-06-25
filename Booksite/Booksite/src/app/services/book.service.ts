import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BookDetails } from 'src/app/models/books.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class BookService {

  endpoint = 'api/book';
  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  };
  constructor(private http: HttpClient) { }

  static isValidIsbn(isbn: string): boolean {
    isbn = isbn.trim();
    const isDigitsOnly = /^[0-9]{13}$/.test(isbn);
    if (!isDigitsOnly)
      return false;

    let i: number;
    let checkDigit: number = 0;
    for (i = 1; i < 13; i++) {
      checkDigit += (+isbn.substring(i - 1, i) * ((i % 2) > 0 ? 1 : 3));
    }

    checkDigit %= 10;
    if (checkDigit != 0) {
      checkDigit = 10 - checkDigit;
    }
    return +isbn.substring(12, 13) === checkDigit;
  }

  getBookDetails(isbn: string): Observable<BookDetails> {
    return this.http.get<BookDetails>(this.endpoint + `?isbn=${isbn}`);
  }

  editBookDetails(bookDetails: BookDetails,
    isAdding: boolean): Observable<BookDetails> {
    if (isAdding) {
      return this.http.post<BookDetails>(this.endpoint + '/add',
        bookDetails, this.httpOptions);
    } else {
      return this.http.post<BookDetails>(this.endpoint + '/edit',
        bookDetails, this.httpOptions);
    }
  }

  getGenres(): Observable<string[]> {
    return this.http.get<string[]>(this.endpoint + `/genres`);
  }
}
