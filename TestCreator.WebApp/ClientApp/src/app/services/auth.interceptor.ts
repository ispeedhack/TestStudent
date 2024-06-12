import { Injector, Injectable } from "@angular/core";
import { HttpHandler, HttpInterceptor, HttpEvent, HttpRequest } from "@angular/common/http";
import { Observable } from "rxjs";
import { map, catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private injector: Injector) { }

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler)
    : Observable<HttpEvent<any>> {

    var auth = this.injector.get(AuthService);
    var token = (auth.isLoggedIn()) ? auth.getAuth()!.token : null;

    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }
    return next.handle(request);
  }
}
