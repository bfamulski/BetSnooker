import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home';
import { BetsComponent } from './bets';
import { LoginComponent } from './login';
import { RegisterComponent } from './register';

import { BasicAuthInterceptor, ErrorInterceptor, DashboardItemRoundFilterPipe } from './_helpers';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    BetsComponent,
    LoginComponent,
    RegisterComponent,
    DashboardItemRoundFilterPipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: BasicAuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
