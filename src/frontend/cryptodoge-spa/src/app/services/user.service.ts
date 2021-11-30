import { TokenDto } from './../../../generated/client/model/tokenDto';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from 'generated/client';
import { BehaviorSubject } from 'rxjs';
import { finalize, map } from 'rxjs/operators';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  jwtHelper = new JwtHelperService();
  public currentUser: User;
  public decodedToken: any;

  constructor(private authApiService: AuthService) {
    if (this.isLoggedIn()) {
      this.decodedToken = this.jwtHelper.decodeToken(
        localStorage.getItem('access_token')
      );
      this.currentUser = {
        id: this.decodedToken.id,
        email: this.decodedToken.email,
        name: this.decodedToken.name,
        role: this.decodedToken.role,
      };
      this.$isLoggedIn.next(true);
    }
  }

  $isLoggedIn = new BehaviorSubject<boolean>(this.isLoggedIn());

  login(email: string, password: string) {
    return this.authApiService
      .authLogin({ emailAddress: email, password: password })
      .pipe(
        map((result) => {
         this.setTokens(result);
        })
      );
  }

  setTokens(result: TokenDto) {
    localStorage.setItem('access_token', result.accessToken);
    localStorage.setItem('refresh_token', result.refreshToken);
    const decodedToken = this.jwtHelper.decodeToken(result.accessToken);          
    this.currentUser = {
      id: decodedToken.id,
      email: decodedToken.email,
      name: decodedToken.name,
      role: decodedToken.role,
    };
    this.$isLoggedIn.next(true);
  }

  logout() {
    return this.authApiService.authLogout()
      .pipe(
        finalize(() => {
          localStorage.removeItem('access_token');
          localStorage.removeItem('refresh_token');  
          this.$isLoggedIn.next(false);
        })
      );
  }

  isLoggedIn() {
    let token = localStorage.getItem('access_token');
    return !!token;    
  }

  hasRole(allowedRoles: string[]): boolean {

    if (!this.isLoggedIn()) {return false;}

    if (allowedRoles.indexOf(this.currentUser.role) === -1) {
      return false;
    }
    
    return true;
  }
}