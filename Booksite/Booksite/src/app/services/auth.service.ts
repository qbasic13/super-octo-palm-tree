import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { sha3_512 } from '@noble/hashes/sha3';
import FingerprintJS from "@fingerprintjs/fingerprintjs";
import { bytesToHex } from '@noble/hashes/utils';
import { AuthReq, AuthRes, RegReq } from 'src/app/models/auth.model';

const endpoint = 'api/auth'
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private fp = FingerprintJS.load();
  constructor(private http: HttpClient) { }

  fingerprint = this.getFingerprint().then(
    resFingerprint => { return resFingerprint },
    error => {return null }
  );

  signIn(authReq: AuthReq): Observable<any> {
    authReq.password = bytesToHex(sha3_512(authReq.password));
    return this.http.post(endpoint + '/signin', authReq, httpOptions);
  }
  signUp(regReq: RegReq): Observable<any> {
    regReq.password = bytesToHex(sha3_512(regReq.password));
    return this.http.post(endpoint + '/signup', regReq, httpOptions);
  }

  refresh(fingerprint: string): Observable<AuthRes> {
    return this.http.post<AuthRes>(endpoint + `/refresh?fingerprint=${fingerprint}`, httpOptions);
  }

  verifyEmail(email: string): Observable<AuthRes> {
    return this.http.post<AuthRes>(endpoint + `/verify?email=${email}`, httpOptions);
  }

  signOut() {
    this.http.post(endpoint + '/signout', httpOptions).subscribe(
      (response) => {
        const logoutRes = response as AuthRes;
        if (logoutRes.isSuccess) {
          localStorage.removeItem('email');
          localStorage.removeItem('access');
          localStorage.removeItem('role');
        }
        return logoutRes.isSuccess;
      },
      (error) => {
        return false;
      }
    );
  }

  parseJwt(token: string) {
    try {
      return JSON.parse(atob(token.split('.')[1]));
    } catch (e) {
      return null;
    }
  }

  getUserEmail() {
    const access = localStorage.getItem('access');
    if (!access)
      return null;
    const body = this.parseJwt(access);
    if (!body)
      return null;
    return body.email;
  }

  saveData(access: string) {
    localStorage.setItem('access', access);
    const jwtPayload = this.parseJwt(access);
    localStorage.setItem('email', jwtPayload.email);
    localStorage.setItem('role', jwtPayload.role);
  }

  hasRole(...params: string[]) {
    const userRole = localStorage.getItem('role');
    if (!userRole)
      return false;
    const hasOneOfRequestedRoles = params.includes(userRole);
    return hasOneOfRequestedRoles;
  }

  isLoggedIn() {
    return localStorage.getItem('email') !== null;
  }

  async getFingerprint(): Promise<string> {
    const fingerprint = localStorage.getItem('fingerprint');
    if (fingerprint === null) {
      const { visitorId } = await (await this.fp).get();
      localStorage.setItem('fingerprint', visitorId);
      return visitorId;
    } else {
      return fingerprint;
    }
  }
}
