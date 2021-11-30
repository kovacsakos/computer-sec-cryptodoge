import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { UserService } from '../services/user.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from 'generated/client';
import { catchError, filter, switchMap, take } from 'rxjs/operators';

@Injectable()
export class RenewTokenInterceptor implements HttpInterceptor {

    jwtHelper = new JwtHelperService();

    private isRefreshing = false;
    private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

    constructor(private userService: UserService, private authService: AuthService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(error => {
            if (error instanceof HttpErrorResponse && error.status === 401) {
              return this.handle401Error(request, next);
            } else {
              return throwError(error);
            }
          }));
    }

    private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
        if (!this.isRefreshing) {
          this.isRefreshing = true;
          this.refreshTokenSubject.next(null);
      
          return this.authService.authRenewToken(localStorage.getItem("refresh_token")).pipe(
            switchMap((token) => {
              this.userService.setTokens(token);
              this.isRefreshing = false;
              this.refreshTokenSubject.next(token.accessToken);
              return next.handle(this.addToken(request, token.accessToken));
            }));
      
        } else {
          return this.refreshTokenSubject.pipe(
            filter(token => token != null),
            take(1),
            switchMap(jwt => {
              return next.handle(this.addToken(request, jwt));
            }));
        }
    }

    private addToken(request: HttpRequest<any>, token: string) {
    return request.clone({
        setHeaders: {
        'Authorization': `Bearer ${token}`
        }
    });
    }
}