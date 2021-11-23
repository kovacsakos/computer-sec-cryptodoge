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
import { SearchComponent } from './components/search/search.component';
import { HomeComponent } from './components/home/home.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    SearchComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    MenubarModule,
    AutoCompleteModule,
    CardModule
  ],
  providers: [{ provide: BASE_PATH, useValue: environment.baseUrl }],
  bootstrap: [AppComponent]
})
export class AppModule { }
