import { HttpClient, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BookCoverService {
  constructor(private http: HttpClient) { }
  endpoint: string = 'api/book/upload';

  uploadCover(file: File, isbn: string): Observable<HttpEvent<any>> {
    const formData: FormData = new FormData();
    formData.append('isbn', isbn);
    formData.append('file', file);
    const request = new HttpRequest('POST',
        this.endpoint, formData, {
      reportProgress: true,
      responseType: 'json'
    });
    return this.http.request(request);
  }
}
