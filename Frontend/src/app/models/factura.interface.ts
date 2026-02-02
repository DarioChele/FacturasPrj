export interface Factura {
  id?: number;
  numeroFactura: string;
  clienteId: number;  
  usuarioId: number;  
  fecha: Date | string;
  montoTotal: number;
  estadoPago: number;
  estadoFactura: number;
  
  // Propiedades opcionales del DTO para mostrar en tablas  
  clienteIdentificacion?: string;
  clienteNombre?: string;
  usuarioNombre?: string;
  estadoPagoDescripcion?: string;
  estadoFacturaDescripcion?: string;

  detalles: DetalleFactura[];
  pagos: PagoFactura[];
}

export interface DetalleFactura {
  facturaId?: number;  
  productoId: number;
  productoNombre?: string; // Para mostrar en la tabla de la factura
  cantidad: number;
  precioUnitario: number;
  precioTotal: number;
}

export interface PagoFactura {
    facturaId?: number;  
    formaPagoId: number;
    formaPagoNombre?: string;
    valorPagado: number;
}