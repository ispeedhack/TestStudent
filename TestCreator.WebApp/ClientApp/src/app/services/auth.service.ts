
import {throwError as observableThrowError,  Observable } from 'rxjs';
import { EventEmitter, Inject, Injectable, PLATFORM_ID } from "@angular/core";
import { isPlatformBrowser } from "@angular/common";
//returns true for browser
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { map, catchError } from 'rxjs/operators';
import { TokenResponse } from 'src/app/interfaces/token.response';
import { TokenRequest } from 'src/app/interfaces/token.request';


@Injectable()
export class AuthService {
  authKey: string = "auth";
  clientId: string = "TestCreator";

  constructor(private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: any) {
  }

  login(username: string, password: string): Observable<boolean>{
    var url = "api/token/auth";
    var data =  {
      Username: username,
      Password: password,
      ClientId: this.clientId,
      GrantType: "password",
      Scope: "offline_access profile email",
      RefreshToken: null
    };

    return this.getAuthFromServer(url, data);
  }

  logout() : boolean {
    this.setAuth(null);
    return true;
  }

  getAuthFromServer(url: string, data: TokenRequest) : Observable<boolean>{
    return this.http.post<TokenResponse>(url, data).pipe(map((result) => {
      let token = result && result.token;

      if (token) {
        this.setAuth(result);

        return true;
      }

      return observableThrowError('Unauthorized');
    }), catchError((err, caught) => { return new Observable<any>(err) }));
  }

  setAuth(auth: TokenResponse | null) : boolean {
    if (isPlatformBrowser) {
      if (auth) {
        localStorage.setItem(this.authKey, JSON.stringify(auth));
      } else {
        localStorage.removeItem(this.authKey);
      }
    }
    return true;
  }

  getAuth(): TokenResponse | null {
    if (isPlatformBrowser(this.platformId)) {
      var item = localStorage.getItem(this.authKey);
      if (item) {
        return JSON.parse(item);
      }
    }
    return null;
  }

  isLoggedIn() {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(this.authKey) != null;
    }
    return false;
  }

  refreshToken() : Observable<boolean> {
    var url = "api/token/auth";
    var data = {
      Username: null,
      Password: null,
      ClientId: this.clientId,
      GrantType: "refresh_token",
      Scope: "offline_access profile email",
      RefreshToken: this.getAuth()!.refreshToken
    };

    return this.getAuthFromServer(url, data);
  }
}
