
import { UserMainPageService } from 'src/app/services/user-main-page.service';
import { ToastService } from './../../services/toast.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  email: string = "";
  password: string = "";

  constructor(private authService: UserService, private router: Router, private toast: ToastService, private mainPageService: UserMainPageService) { }

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(this.mainPageService.getMainPageForUser());
    }
  }

  login() {
    if (this.loginDisabled()) {return;}
    this.authService.login(this.email, this.password).subscribe(next => {
      this.toast.success('Login successful');
    }, error => {
      this.toast.error('Login failed');
    }, () => {
      if (this.authService.hasRole(['ADMIN'])) {
        this.router.navigate(this.mainPageService.getMainPageForUser());
      }
      else {
        this.router.navigate(this.mainPageService.getMainPageForUser());
      }
    });
  }

  loginDisabled(): boolean {
    return this.email.length === 0 || this.password.length === 0;
  }

}
