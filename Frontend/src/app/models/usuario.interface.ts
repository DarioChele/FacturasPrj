export interface Usuario {
  id?: number;              // '?' porque al crear no tiene ID
  nombre: string;
  passwordHash: string;
  rol: number;
  rolDescripcion?: string;
  estado: number;           // 1 para Vendedor, 0 para Admin
  estadoDescripcion?: string; // Solo viene en el DTO (opcional)
}