import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
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
  
  constructor(
    private proveedorService: ProveedorService, 
    private productoService: ProductoService,
    private auth: AuthService,
    private cdr: ChangeDetectorRef
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
  }
  // Método para gestionar los pagos antes de guardar
  prepararPagos() {
    // Si la lista de pagos está vacía, agregamos el total con la forma de pago seleccionada
    if (this.producto.proveedores.length === 0) {
      
    }
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
    this.productoService.crear(this.producto).subscribe({
      
      
      next: (res) => {
        alert('Producto creado con éxito');
        // Redirigir al listado para que se vea "ligero" el flujo
        // this.router.navigate(['/facturas']); 
      },
      error: (err) => {
        alert('Error al guardar: ' + (err.error || 'Intente nuevamente'));
      }
    });
  }
}