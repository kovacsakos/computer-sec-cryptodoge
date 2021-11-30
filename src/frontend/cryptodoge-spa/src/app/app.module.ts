import { CommentComponent } from './components/comment/comment.component';
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
import {ConfirmPopupModule} from 'primeng/confirmpopup';
import {ConfirmationService, MessageService} from 'primeng/api';
import {FileUploadModule} from 'primeng/fileupload';
import {PaginatorModule} from 'primeng/paginator';
import {PanelModule} from 'primeng/panel';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CaffAnimatorComponent } from './components/caff-animator/caff-animator.component';
import { ButtonModule } from 'primeng/button';
import { JwtModule } from '@auth0/angular-jwt';
import { UserMainPageService } from './services/user-main-page.service';
import {OverlayPanelModule} from 'primeng/overlaypanel';
import { UserService } from './services/user.service';
import {InplaceModule} from 'primeng/inplace';
import { ToastModule } from 'primeng/toast';
import { httpInterceptorProviders } from './interceptors/interceptor-providers';
import { RecaptchaFormsModule, RecaptchaModule, RecaptchaSettings, RECAPTCHA_SETTINGS } from 'ng-recaptcha';
import { DetailsComponent } from './components/details/details.component';

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
    DetailsComponent,
    CommentComponent,
    CaffAnimatorComponent,
    UploadComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ConfirmPopupModule,
    HttpClientModule,
    FormsModule,
    MenubarModule,
    ToastModule,
    InplaceModule,
    OverlayPanelModule,
    AutoCompleteModule,
    InputTextModule,
    CardModule,
    PaginatorModule,
    RecaptchaModule,
    RecaptchaFormsModule,
    PanelModule,
    ReactiveFormsModule,
    ButtonModule,
    BrowserAnimationsModule,
    FileUploadModule,
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
    ConfirmationService,
    httpInterceptorProviders,
    { provide: BASE_PATH, useValue: environment.baseUrl },
    {
      provide: RECAPTCHA_SETTINGS,
      useValue: {
        siteKey: environment.recaptcha.siteKey,
      } as RecaptchaSettings,
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
