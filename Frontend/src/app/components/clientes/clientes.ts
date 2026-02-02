import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ClienteService } from '../../services/cliente.service';
import { Cliente } from '../../models/cliente.interface';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-clientes',
  standalone: false,
  templateUrl: './clientes.html',
  styleUrl: './clientes.css',
})
export class Clientes implements OnInit {
  // 1. Definimos las propiedades que usará el HTML
  listaClientes: Cliente[] = [];
  mostrarModal = false;
  //clienteTmp: any = {};
  clienteTmp: Cliente = {} as Cliente;

  // 2. EL CONSTRUCTOR: Aquí es donde "nace" ClienteService
  constructor(
              private clienteService: ClienteService, 
              public auth: AuthService, 
              private cdr: ChangeDetectorRef
            ) {}

  // 3. ngOnInit: Se ejecuta cuando la página carga
  ngOnInit(): void {
    this.cargarClientes();
  }

  // 4. FUNCIÓN cargarClientes
  cargarClientes() {
    this.clienteService.getTodos().subscribe({
      next: (res) => {
        this.listaClientes = res; // <--- ESTO es lo que llena la tabla
        this.cdr.detectChanges();
      },
      error: (err: any) => { // Le ponemos ': any' para que TypeScript no se queje xD
      }
    });
  }

  nuevoCliente() {
    this.clienteTmp = {} as Cliente; 
    this.mostrarModal = true;
  }
  abrirModal(cliente?: Cliente) {
    if (cliente) {
      // EDITAR: Creamos una copia para no modificar la tabla antes de guardar
      this.clienteTmp = { ...cliente };
    } else {
      // NUEVO: Objeto limpio
      this.clienteTmp = {} as Cliente;
    }
    this.mostrarModal = true;
  }
  
  guardar() {
    if (this.clienteTmp.id) {
      // Si tiene ID, actualizamos
      this.clienteService.actualizar(this.clienteTmp).subscribe({
        next: () => this.finalizarGuardado('Cliente actualizado'),
        error: (err: any) => alert(err.message)
      });
    } else {
      // Si no tiene ID, creamos
      this.clienteService.crear(this.clienteTmp).subscribe({
        next: () => this.finalizarGuardado('Cliente creado'),
        error: (err: any) => alert(err.message)
      });
    }
  }
  finalizarGuardado(mensaje: string) {
    alert(mensaje);
    this.mostrarModal = false;
    this.cargarClientes();
  }
  borrar(id: number | undefined) {
    if (!this.auth.esAdmin()) {
      alert('No tienes permisos para eliminar clientes.');
      return;
    }

    if (id && confirm('¿Estás seguro de eliminar este cliente?')) {
      this.clienteService.eliminar(id).subscribe(() => this.cargarClientes());
    }
  }
}