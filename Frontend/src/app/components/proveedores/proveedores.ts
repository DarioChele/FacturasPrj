import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ProveedorService } from '../../services/proveedor.service';
import { Proveedor } from '../../models/proveedor.interface';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-proveedores',
  standalone: false,
  templateUrl: './proveedores.html',
  styleUrl: './proveedores.css',
})
export class Proveedores implements OnInit {
  // 1. Definimos las propiedades que usará el HTML
  listaProveedores: Proveedor[] = [];
  mostrarModal = false;
  //ProveedorTmp: any = {};
  ProveedorTmp: Proveedor = {} as Proveedor;

  // 2. EL CONSTRUCTOR: Aquí es donde "nace" ProveedorService
  constructor(
              private ProveedorService: ProveedorService, 
              public auth: AuthService, 
              private cdr: ChangeDetectorRef
            ) {}

  // 3. ngOnInit: Se ejecuta cuando la página carga
  ngOnInit(): void {
    this.cargarProveedores();
  }

  // 4. FUNCIÓN cargarProveedors
  cargarProveedores() {
    this.ProveedorService.getTodos().subscribe({
      next: (res) => {
        this.listaProveedores = res; // <--- ESTO es lo que llena la tabla
        this.cdr.detectChanges();
      },
      error: (err: any) => { // Le ponemos ': any' para que TypeScript no se queje xD
      }
    });
  }

  nuevoProveedor() {
    this.ProveedorTmp = {} as Proveedor; 
    this.mostrarModal = true;
  }
  abrirModal(Proveedor?: Proveedor) {
    if (Proveedor) {
      // EDITAR: Creamos una copia para no modificar la tabla antes de guardar
      this.ProveedorTmp = { ...Proveedor };
    } else {
      // NUEVO: Objeto limpio
      this.ProveedorTmp = {} as Proveedor;
    }
    this.mostrarModal = true;
  }
  
  guardar() {
    if (this.ProveedorTmp.id) {
      // Si tiene ID, actualizamos
      this.ProveedorService.actualizar(this.ProveedorTmp).subscribe({
        next: () => this.finalizarGuardado('Proveedor actualizado'),
        error: (err: any) => alert(err.message)
      });
    } else {
      // Si no tiene ID, creamos
      this.ProveedorService.crear(this.ProveedorTmp).subscribe({
        next: () => this.finalizarGuardado('Proveedor creado'),
        error: (err: any) => alert(err.message)
      });
    }
  }
  finalizarGuardado(mensaje: string) {
    alert(mensaje);
    this.mostrarModal = false;
    this.cargarProveedores();
  }
  borrar(id: number | undefined) {
    if (!this.auth.esAdmin()) {
      alert('No tienes permisos para eliminar Proveedors.');
      return;
    }

    if (id && confirm('¿Estás seguro de eliminar este Proveedor?')) {
      this.ProveedorService.eliminar(id).subscribe(() => this.cargarProveedores());
    }
  }
}