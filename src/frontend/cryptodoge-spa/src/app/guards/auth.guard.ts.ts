import { UserMainPageService } from './../services/user-main-page.service';
import { UserService } from 'src/app/services/user.service';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { ToastService } from '../services/toast.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: UserService,
    private router: Router,
    private toaster: ToastService,
    private mainPageService: UserMainPageService
  ) {}

  canActivate(next: ActivatedRouteSnapshot): boolean {
    const roles = next.firstChild.data.roles as Array<string>;
    if (roles) {
      
      if (this.authService.hasRole(roles)) {
        return true;
      }
      else {
        this.toaster.error('Login to view this page');
        this.router.navigate(this.mainPageService.getMainPageForUser());    
          return false;
      }
    }

    if (this.authService.isLoggedIn()) {
      return true;
    }

    this.toaster.error('Login to view this page');
    this.router.navigate(['']);
    return false;
  }
}
