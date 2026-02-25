export interface Producto {
  id: number;
  nombre: string;
  precioUnitario: number;
  estado: number;
  estadoDescripcion?: string;
  stockTotal?: number;
  proveedores: DetalleProveedor[];
}

export interface DetalleProveedor {
  proveedorId: number;
  nombreProveedor: string;
  precio: number;
  stock: number;
  numeroLote: string;
}