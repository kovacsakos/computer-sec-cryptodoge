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

  ngOnInit() {
    this.items = [
      {
        label: 'Home',
        icon: 'pi pi-fw pi-home',
        routerLink: ['/']
      },
      {
        icon: 'pi pi-fw pi-search',
        label: 'Search',
        routerLink: ['/search']
      },
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
      }
    ];
  }
}
