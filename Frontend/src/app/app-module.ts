import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './interceptors/auth.interceptor';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { Login } from './components/login/login';
import { Clientes } from './components/clientes/clientes';
import { Productos } from './components/productos/productos';
import { Usuarios } from './components/usuarios/usuarios';
import { Facturas } from './components/facturas/facturas';
import { NuevaFactura } from './components/nueva-factura/nueva-factura';

@NgModule({
  declarations: [
    App,
    Login,
    Clientes,
    Productos,
    Usuarios,
    Facturas,
    NuevaFactura
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),provideHttpClient(withInterceptors([authInterceptor])),
  ],
  bootstrap: [App]
})
export class AppModule { }
