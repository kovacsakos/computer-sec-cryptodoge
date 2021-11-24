import { Component } from '@angular/core';
import { MenubarModule } from 'primeng/menubar';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'cryptodoge-spa';

  items: MenuItem[];
  isLoggedIn: boolean;

  ngOnInit() {
    this.items = [
      {
        label: 'Home',
        icon: 'pi pi-fw pi-home',
        routerLink: ['']
      }
    ];

    if (this.isLoggedIn) { this.items.push(
      {
        label: 'Profile',
        icon: 'pi pi-fw pi-user',
        routerLink: ['/profile']
      })}
    else {
      this.items.push(
        {
          label: 'Profile',
          icon: 'pi pi-fw pi-user',
          items: [{
            label: 'Login',
            routerLink: ['/login']
          },
          {
            label: 'Register',
            routerLink: ['/register']
          }]
        })
    }
  }
}
