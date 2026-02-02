import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';

import { Login } from './components/login/login';
import { Clientes } from './components/clientes/clientes';
import { Productos } from './components/productos/productos';
import { Usuarios } from './components/usuarios/usuarios';
import { Facturas } from './components/facturas/facturas';
import { NuevaFactura } from './components/nueva-factura/nueva-factura';

const routes: Routes = [
  {path: 'login', component: Login},
  {path: 'clientes', component: Clientes, canActivate: [authGuard]},
  {path: 'productos', component: Productos, canActivate: [authGuard]},
  {path: 'usuarios', component: Usuarios, canActivate: [authGuard]},
  {path: 'facturas', component: Facturas, canActivate: [authGuard]},
  {path: 'nueva-factura', component: NuevaFactura, canActivate: [authGuard]},
  {path: '', redirectTo: '/login', pathMatch: 'full' },
  // Esta es la ruta "comodín" por si escriben cualquier cosa
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
