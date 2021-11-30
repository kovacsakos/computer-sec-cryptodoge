import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { BASE_PATH } from 'generated/client';
import { MenubarModule } from 'primeng/menubar';
import {CardModule} from 'primeng/card';
import {InputTextModule} from 'primeng/inputtext';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomeComponent } from './components/home/home.component';
import { ProfileComponent } from './components/profile/profile.component';
import {PaginatorModule} from 'primeng/paginator';
import {PanelModule} from 'primeng/panel';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CaffAnimatorComponent } from './components/caff-animator/caff-animator.component';
import { ButtonModule } from 'primeng/button';
import { JwtModule } from '@auth0/angular-jwt';
import { MessageService } from 'primeng/api';
import { UserMainPageService } from './services/user-main-page.service';
import { UserService } from './services/user.service';
import { ToastModule } from 'primeng/toast';
import { httpInterceptorProviders } from './interceptors/interceptor-providers';

export function tokenGetter() {
  return localStorage.getItem("access_token");
}


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    ProfileComponent,
    CaffAnimatorComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    MenubarModule,
    ToastModule,
    AutoCompleteModule,
    InputTextModule,
    CardModule,
    PaginatorModule,
    PanelModule,
    ReactiveFormsModule,
    ButtonModule,
    BrowserAnimationsModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: [environment.baseUrl.replace("http://", "").replace("https://", "")],
        disallowedRoutes: [
          `${environment.baseUrl}/api/Auth/login`,
          `${environment.baseUrl}/api/Auth/register`,
          `${environment.baseUrl}/api/Auth/renew`,
        ],
      },
    }),
  ],
  providers: [
    MessageService,
    UserMainPageService,
    UserService,
    httpInterceptorProviders,
    { provide: BASE_PATH, useValue: environment.baseUrl }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
