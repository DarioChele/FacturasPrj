import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ProductoService } from '../../services/producto.service';
import { Producto } from '../../models/producto.interface';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-productos',
  standalone: false,
  templateUrl: './productos.html',
  styleUrl: './productos.css',
})
export class Productos implements OnInit {
  // 1. Definimos las propiedades que usará el HTML
  listaProductos: Producto[] = [];
  mostrarModal = false;
  //productoTmp: any = {};
  productoTmp: Producto = {} as Producto;

  // 2. EL CONSTRUCTOR: Aquí es donde "nace" ProductoService
  constructor(
              private productoService: ProductoService, 
              public auth: AuthService, 
              private cdr: ChangeDetectorRef
            ) {}

  // 3. ngOnInit: Se ejecuta cuando la página carga
  ngOnInit(): void {
    this.cargarProductos();
  }

  // 4. FUNCIÓN cargarProductos
  cargarProductos() {
    this.productoService.getTodos().subscribe({
      next: (res) => {        
        this.listaProductos = res; // <--- ESTO es lo que llena la tabla
        this.cdr.detectChanges();
      },
      error: (err: any) => { // Le ponemos ': any' para que TypeScript no se queje xD
        console.error('Error al traer productos', err);
      }
    });
  }

  nuevoProducto() {
    this.productoTmp = {} as Producto;
    this.mostrarModal = true;
  }
  abrirModal(producto?: Producto) {
    if (producto) {
      // EDITAR: Creamos una copia para no modificar la tabla antes de guardar
      this.productoTmp = { ...producto };
    } else {
      // NUEVO: Objeto limpio
      this.productoTmp = {} as Producto;
    }
    this.mostrarModal = true;
  }
  
  guardar() {
    if (this.productoTmp.id) {
      // Si tiene ID, actualizamos
      this.productoService.actualizar(this.productoTmp).subscribe({
        next: () => this.finalizarGuardado('Producto actualizado'),
        error: (err: any) => alert(err.message)
      });
    } else {
      // Si no tiene ID, creamos
      this.productoService.crear(this.productoTmp).subscribe({
        next: () => this.finalizarGuardado('Producto creado'),
        error: (err: any) => alert(err.message)
      });
    }
  }
  finalizarGuardado(mensaje: string) {
    alert(mensaje);
    this.mostrarModal = false;
    this.cargarProductos();
  }
  borrar(id: number | undefined) {
    if (!this.auth.esAdmin()) {
      alert('No tienes permisos para eliminar productos.');
      return;
    }

    if (id && confirm('¿Estás seguro de eliminar este producto?')) {
      this.productoService.eliminar(id).subscribe(() => this.cargarProductos());
    }
  }
}