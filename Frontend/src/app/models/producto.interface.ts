export interface Producto {
  id?: number;              // '?' porque al crear no tiene ID
  nombre: string;
  precioUnitario: number;   // Usamos camelCase para coincidir con el JSON
  estado: number;           // 1 para Activo, 0 para Inactivo
  estadoDescripcion?: string; // Solo viene en el DTO (opcional)
}