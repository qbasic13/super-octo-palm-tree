import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ProfileRes } from '../models/profile.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  endpoint = 'api/profile';

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  };

  constructor(private http: HttpClient,
   private authService: AuthService) { }

  getProfileDetails(email: string): Observable<ProfileRes> {
    const userEmail = email.length > 0
      ? email : this.authService.getUserEmail();
    if (!userEmail)
      return of<ProfileRes>({
        isSuccess: false,
        status: 'not_signed_in',
        message: "Not signed in"
      });
    return this.http.get<ProfileRes>(
      this.endpoint, this.httpOptions);
  }
}
