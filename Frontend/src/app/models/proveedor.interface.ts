export interface Proveedor {
  id?: number;
  identificacion: string;
  nombre: string;
  telefono: string;
  correo: string;
  estado?: number;
  estadoDescripcion?: string;
}