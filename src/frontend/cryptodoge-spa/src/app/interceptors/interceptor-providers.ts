
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { RenewTokenInterceptor } from './renew-token-interceptor';


/** Http interceptor providers in outside-in order */
export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: RenewTokenInterceptor, multi: true },
];