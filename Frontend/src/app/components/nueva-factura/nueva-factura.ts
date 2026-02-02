import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FacturaService } from '../../services/factura.service';
import { Factura } from '../../models/factura.interface';
import { AuthService } from '../../services/auth.service';
import { Cliente } from '../../models/cliente.interface';
import { Producto } from '../../models/producto.interface';
import { ClienteService } from '../../services/cliente.service';
import { ProductoService } from '../../services/producto.service';
import { FormaPago } from '../../models/forma-pago.interface';

@Component({
  selector: 'app-nueva-factura',
  standalone: false,
  templateUrl: './nueva-factura.html',
  styleUrl: './nueva-factura.css',
})
export class NuevaFactura implements OnInit {
  // Datos maestros para los Selects
  clientes: Cliente[] = [];
  productos: Producto[] = [];
  vendedorNombre: string = '';
 
  // El objeto que enviaremos al Backend (según estructura)
   factura: Factura = {
     numeroFactura: '',
     clienteId: 0,
     usuarioId: 0, // Se sacará del Token
     fecha: new Date(),
     montoTotal: 0,
     estadoPago: 1,
     estadoFactura: 1,
     detalles: [],
     pagos: []
   };
  
  formasPago: FormaPago[] = [];
  // Variables auxiliares para la línea actual
  itemTmp = { productoId: 0, cantidad: 1, precio: 0, nombre: '' };
  pagoTmp = { formaPagoId: 1, valor: 0 };
  totalPagado: number = 0;
  saldoPendiente: number = 0;

  constructor(
    private clienteService: ClienteService, 
    private productoService: ProductoService,
    private auth: AuthService,
    private facturaService: FacturaService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    // 1. Datos del vendedor
    const tokenData = this.auth.getDecodedToken(); 
    this.vendedorNombre = tokenData['name'] || tokenData['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    this.factura.usuarioId = tokenData['nameidentifier'] || tokenData['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    
    this.clienteService.getTodos("1").subscribe({
      next: (res) => {
        this.clientes = res;
        this.cdr.detectChanges(); // <--- recarga la vista
      },
      error: (err) => console.error('Error clientes:', err)
    });

    // 3. Carga de Productos
    this.productoService.getTodos("1").subscribe({
        next: (res) => {
          this.productos = res;
          this.cdr.detectChanges(); // <--- Y AQUÍ TAMBIÉN
        },
        error: (err) => console.error('Error productos:', err)
      });
    // 4. Carga de Formas de Pago
    this.facturaService.getFormasPago().subscribe({
        next: (res) => {
          this.formasPago = res;
          this.cdr.detectChanges(); // <--- Y AQUÍ TAMBIÉN
        },
        error: (err) => console.error('Error formas de pago:', err)
      });
  }
  // Método para gestionar los pagos antes de guardar
  prepararPagos() {
    // Si la lista de pagos está vacía, agregamos el total con la forma de pago seleccionada
    if (this.factura.pagos.length === 0) {
      this.factura.pagos.push({
        facturaId: 0,
        formaPagoId: this.pagoTmp.formaPagoId,
        valorPagado: this.factura.montoTotal
      });
    }
  }
  // --- Lógica de Cálculos ---
  agregarDetalle() {
    const prod = this.productos.find(p => p.id == this.itemTmp.productoId);
    if (!prod || this.itemTmp.cantidad <= 0) return;

    // BUSCAR SI EL PRODUCTO YA EXISTE EN EL DETALLE
    const existente = this.factura.detalles.find(d => d.productoId === prod.id);

    if (existente) {
      // Si ya existe, actualizamos la cantidad y el total de esa fila
      existente.cantidad += this.itemTmp.cantidad;
      existente.precioTotal = existente.cantidad * existente.precioUnitario;
    } else {
      // Si es nuevo, lo agregamos normal
      this.factura.detalles.push({
        productoId: prod.id!,
        cantidad: this.itemTmp.cantidad,
        precioUnitario: prod.precioUnitario,
        precioTotal: this.itemTmp.cantidad * prod.precioUnitario,
        productoNombre: prod.nombre 
      } as any);
    }

    this.calcularTotales();
    this.itemTmp = { productoId: 0, cantidad: 1, precio: 0, nombre: '' };
  }
  // Método para calcular cuánto falta por pagar
  recalcularSaldos() {
    this.totalPagado = this.factura.pagos.reduce((acc, p) => acc + p.valorPagado, 0);
    this.saldoPendiente = this.factura.montoTotal - this.totalPagado;    
    // Por defecto, sugerimos pagar el saldo pendiente en el siguiente pago
    this.pagoTmp.valor = this.saldoPendiente > 0 ? this.saldoPendiente : 0;
  }
  agregarPago() {
    if (this.pagoTmp.valor <= 0) return;
    if (this.pagoTmp.valor > this.saldoPendiente) {
      alert('El valor del pago no puede ser mayor al saldo pendiente');
      return;
    }
    // BUSCAR SI EL PRODUCTO YA EXISTE EN EL DETALLE
    const existente = this.factura.pagos.find(d => d.formaPagoId === this.pagoTmp.formaPagoId);

    if (existente) {
      // Si ya existe, actualizamos el valor pagado de esa forma
      existente.valorPagado += this.pagoTmp.valor;
    } else {
      // Si es nuevo, lo agregamos normal

      ////////////////////////////////
      this.factura.pagos.push({
        facturaId: 0,
        formaPagoId: Number(this.pagoTmp.formaPagoId),
        valorPagado: this.pagoTmp.valor,
        // Para la UI, buscamos el nombre de la forma de pago
        formaPagoNombre: this.formasPago.find(f => f.id == this.pagoTmp.formaPagoId)?.tipoPago 
      } as any);
    }

    this.recalcularSaldos();
  }

  quitarPago(index: number) {
    this.factura.pagos.splice(index, 1);
    this.recalcularSaldos();
  }

  calcularTotales() {
    this.factura.montoTotal = this.factura.detalles.reduce((acc, d) => acc + d.precioTotal, 0);
    this.recalcularSaldos();
  }  
  obtenerInfoCliente(): Cliente {
    // Buscamos el cliente en la lista que ya cargamos en el ngOnInit
    const clienteEncontrado = this.clientes.find(c => c.id == this.factura.clienteId);
    
    // Si lo encuentra, lo retorna; si no, retorna un objeto vacío con strings vacíos
    return clienteEncontrado || { nombre: '', identificacion: '', telefono: '', correo: '' } as Cliente;
  }
  quitarDetalle(index: number) {
    this.factura.detalles.splice(index, 1);
    this.calcularTotales();
  }
  guardar() {
  // VALIDACIONES PREVIAS
  if (this.factura.clienteId == 0) {
    alert('Error: Debe seleccionar un cliente.');
    return;
  }
  if (this.factura.detalles.length === 0) {
    alert('Error: Debe agregar al menos un producto a la factura.');
    return;
  }
  if (!this.factura.numeroFactura) {
    alert('Error: El número de factura es obligatorio.');
    return;
  }

  // AUTO-COMPLETAR PAGO (Si no hay pagos, asumimos efectivo por el total)
  // if (this.factura.pagos.length === 0) {
  //   this.factura.pagos.push({
  //     facturaId: 0,
  //     formaPagoId: 1, // ID de efectivo por defecto
  //     valorPagado: this.factura.montoTotal
  //   });
  // }
  this.recalcularSaldos();
  if (this.saldoPendiente > 0) {
    alert('Error: El total pagado no cubre el monto total de la factura.');
    return;
  }

  this.facturaService.crear(this.factura).subscribe({
    next: (res) => {
      alert('Factura guardada con éxito');
      // Redirigir al listado para que se vea "ligero" el flujo
      // this.router.navigate(['/facturas']); 
    },
    error: (err) => {
      alert('Error al guardar: ' + (err.error || 'Intente nuevamente'));
    }
  });
}
}