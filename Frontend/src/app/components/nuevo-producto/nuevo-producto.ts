import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Producto } from '../../models/producto.interface';
import { Proveedor } from '../../models/proveedor.interface';
import { ProductoService } from '../../services/producto.service';
import { ProveedorService } from '../../services/proveedor.service';

@Component({
  selector: 'app-nuevo-producto',
  standalone: false,
  templateUrl: './nuevo-producto.html',
  styleUrl: './nuevo-producto.css',
})
export class NuevoProducto implements OnInit {
  // Datos maestros para los Selects
  proveedores: Proveedor[]=[];
  vendedorNombre: string = '';
 
  // El objeto que enviaremos al Backend (según estructura)  
  producto: Producto= {
    id:0,
    nombre: '',
    precioUnitario: 0,
    estado: 0,
    proveedores :[]
  };
  // Variables auxiliares para la línea actual
  proveedorTmp = { proveedorId: 0, nombreProveedor: '', precio: 0, stock: 1, numeroLote: '' }; 
  // Variable para definir si está en modo edición o no
  modoEdicion: boolean = false;

  
  constructor(
    private proveedorService: ProveedorService, 
    private productoService: ProductoService,
    private auth: AuthService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    // 1. Datos del vendedor
    const tokenData = this.auth.getDecodedToken(); 
    this.vendedorNombre = tokenData['name'] || tokenData['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    //this.factura.usuarioId = tokenData['nameidentifier'] || tokenData['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    
    this.proveedorService.getTodos("1").subscribe({
      next: (res) => {
        this.proveedores = res;
        this.cdr.detectChanges(); // <--- recarga la vista
      },
      error: (err) => console.error('Error proveedores:', err)
    });
    // Revisar si estamos en modo edición 
    const id = this.route.snapshot.paramMap.get('id'); 
    if (id) { 
      this.modoEdicion = true; 
      this.productoService.getTodos("", id).subscribe({ 
        next: (res) => { 
          this.producto = res[0];
          this.cdr.detectChanges(); // <--- recarga la vista
        }, 
        error: (err) => console.error('Error cargando producto:', err) }
      ); 
    }else{
      this.producto = { id: 0, nombre: '', precioUnitario: 0, estado: 0, proveedores: [] };
    }
  }
  // Método para cargar un producto que se quiera editar
  abrirProducto(producto: Producto) {
    this.producto = { ...producto }; // clonas el objeto
    this.modoEdicion = true;
  }

  // --- Lógica de Cálculos ---
  agregarDetalle() {
    const proveed = this.proveedores.find(p => p.id == this.proveedorTmp.proveedorId);
    if (!proveed || this.proveedorTmp.stock <= 0) return;
    if (!proveed || this.proveedorTmp.precio <= 0) return;
    if (!proveed || this.proveedorTmp.numeroLote === '') return;

    // BUSCAR SI EL PROVEEDOR YA EXISTE EN EL DETALLE
    const existente = this.producto.proveedores.find(d => d.proveedorId === proveed.id);

    if (existente) {
      // Si ya existe, actualizamos la cantidad y el total de esa fila
      existente.stock += this.proveedorTmp.stock;
      //existente.precio = existente.cantidad * existente.precioUnitario;
    } else {
      // Si es nuevo, lo agregamos normal
      this.producto.proveedores.push({
        proveedorId: proveed.id!,
        nombreProveedor: proveed.nombre,        
        precio: this.proveedorTmp.precio,
        stock: this.proveedorTmp.stock,
        numeroLote: this.proveedorTmp.numeroLote
      } as any);
    }
    this.proveedorTmp = { proveedorId: 0, nombreProveedor: '', precio: 0, stock: 1, numeroLote: '' }; 
  }  
  quitarDetalle(index: number) {
    this.producto.proveedores.splice(index, 1);
  }
  guardar() {
    // VALIDACIONES PREVIAS
    if (!this.producto.nombre || this.producto.nombre.trim().length < 1) {
      alert('Error: Debe agregar el nombre del producto.');
      return;
    }
    if (this.producto.precioUnitario <= 0) {
      alert('Error: Debe agregar el PvP del producto.');
      return;
    }
    if (this.producto.proveedores.length < 1) {
      alert('Error: Debe agregar al menos un proveedor.');
      return;
    }   
    if (this.modoEdicion){
      this.productoService.actualizar(this.producto).subscribe({
        next: (res) => {
          alert('Producto actualizado con éxito');          
        },
        error: (err) => {
          alert('Error al actualizar: ' + (err.error || 'Intente nuevamente'));
        }
      });
    }else{
      this.productoService.crear(this.producto).subscribe({
        next: (res) => {
          alert('Producto creado con éxito');
          // this.router.navigate(['/facturas']); 
        },
        error: (err) => {
          alert('Error al guardar: ' + (err.error || 'Intente nuevamente'));
        }
      });
    }
  }
  cambiaEstado() {
  if (this.producto.estado === 1) {
    this.producto.estado = 0;
    this.producto.estadoDescripcion = 'Inactivo';
  } else {
    this.producto.estado = 1;
    this.producto.estadoDescripcion = 'Activo';
  }
}

}