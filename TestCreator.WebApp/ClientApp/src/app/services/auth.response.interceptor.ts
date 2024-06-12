
import {throwError as observableThrowError,  Observable } from 'rxjs';
import { Injector, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import {
  HttpClient, HttpHandler, HttpInterceptor,
  HttpEvent, HttpRequest, HttpResponse, HttpErrorResponse
} from "@angular/common/http";
import { map, catchError, filter, tap, flatMap, mergeMap } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthResponseInterceptor implements HttpInterceptor {

  currentRequest: HttpRequest<any>;
  auth: AuthService;

  constructor(private injector: Injector,
    private router: Router) {

  }


  intercept(request: HttpRequest<any>,
    next: HttpHandler): Observable<HttpEvent<any>> {

    this.auth = this.injector.get(AuthService);
    var token = (this.auth.isLoggedIn()) ? this.auth.getAuth()!.token : null;

    if (token) {
      this.currentRequest = request;

      return next.handle(request).pipe(tap((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          //do nothing
        }
      }), catchError(error => {
        return this.handleError(error, next);
      }));

    } else {
      return next.handle(request);
    }

  }

  handleError(error: any, next: HttpHandler) {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 401) {
        //JWT token is outdated, trying to refresh.
        console.log("Token outdated. Trying to refresh....");

        var previousRequest = this.currentRequest;

        return this.auth.refreshToken().pipe(mergeMap((refreshed) => {
          var token = (this.auth.isLoggedIn()) ? this.auth.getAuth()!.token : null;

          if (token) {
            previousRequest = previousRequest.clone({
              setHeaders: { Authorization: `Bearer ${token}` }
            });
          }
          return next.handle(previousRequest);
        }));
      }
    }
    return observableThrowError(error);
  }
}
