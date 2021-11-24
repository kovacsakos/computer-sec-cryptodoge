import { CaffAnimatorComponent } from './components/caff-animator/caff-animator.component';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { BASE_PATH } from 'generated/client';
import { MenubarModule } from 'primeng/menubar';
import {CardModule} from 'primeng/card';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomeComponent } from './components/home/home.component';
import { ProfileComponent } from './components/profile/profile.component';
import {PaginatorModule} from 'primeng/paginator';
import {PanelModule} from 'primeng/panel';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,
    CaffAnimatorComponent
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    ProfileComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    MenubarModule,
    AutoCompleteModule,
    CardModule,
    PaginatorModule,
    PanelModule,
    BrowserAnimationsModule
  ],
  providers: [{ provide: BASE_PATH, useValue: environment.baseUrl }],
  bootstrap: [AppComponent]
})
export class AppModule { }
