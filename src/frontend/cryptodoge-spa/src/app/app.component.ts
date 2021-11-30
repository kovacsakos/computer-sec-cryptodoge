import { Component, OnInit, OnDestroy } from '@angular/core';
import { MenubarModule } from 'primeng/menubar';
import { MenuItem } from 'primeng/api';
import { UserService } from './services/user.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'cryptodoge-spa';

  items: MenuItem[] = [];
  isLoggedIn: boolean;
  private subscribtion: Subscription;

  constructor(private userService: UserService, private router: Router) {
    this.subscribtion = this.userService.$isLoggedIn.subscribe(data => {
      this.isLoggedIn = data

      if (this.isLoggedIn) {
        this.items = [
          {
            label: 'Home',
            icon: 'pi pi-fw pi-home',
            routerLink: ['']
          }
        ]
      }
  
      else {
        this.items = [
          {
            label: 'Login',
            icon: 'pi pi-fw pi-user',
            routerLink: ['/login']
          },
          {
            label: 'Register',
            icon: 'pi pi-fw pi-user',
            routerLink: ['/register']
          }
        ]
      }
      
    });
  }

  ngOnInit() {
  }

  logout() {
    this.userService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }

  ngOnDestroy(): void {
    this.subscribtion.unsubscribe();
  }

}
