import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { sha3_512 } from '@noble/hashes/sha3';
import FingerprintJS from "@fingerprintjs/fingerprintjs";
import { bytesToHex } from '@noble/hashes/utils';
import { AuthReq, AuthRes, RegReq } from 'src/app/models/auth.model';

const endpoint = "api/auth"
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
    res => { return res },
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

  signOut() {
    this.http.post(endpoint + '/signout', httpOptions).subscribe(
      (response) => {
        const logoutRes = response as AuthRes;
        if (logoutRes.isSuccess) {
          localStorage.removeItem("email");
          localStorage.removeItem("access");
          localStorage.removeItem("role");
        } else {
          console.log("Let me out!");
        }
      },
      (error) => {
        console.log(error + "Let me out!");
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

  async getFingerprint(): Promise<string> {
    const fingerprint = localStorage.getItem("fingerprint");
    if (fingerprint === null) {
      const { visitorId } = await (await this.fp).get();
      localStorage.setItem("fingerprint", visitorId);
      return visitorId;
    } else {
      return fingerprint;
    }
  }
}
