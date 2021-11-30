import { Injectable } from '@angular/core';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root'
})
export class UserMainPageService {

  constructor(private userService: UserService) { }

  getMainPageForUser() {
    if (!this.userService.isLoggedIn()) {
      return [''];
    }

    if (this.userService.hasRole(['ADMIN']))  {
      return ['/home'];
    }
    return ['/home'];
  }


}
