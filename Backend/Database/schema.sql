/*
    sqlite3 facturacion.db < schema.sql
*/

-- Tabla de Clientes
CREATE TABLE Clientes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Identificacion TEXT NOT NULL,
    Nombre TEXT NOT NULL,
    Telefono TEXT,
    Correo TEXT,
    Estado INTEGER NOT NULL DEFAULT 1 -- 1: Activo, 0: Inactivo
);

-- Tabla de Usuarios/Vendedores
CREATE TABLE Usuarios (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    PasswordHash TEXT NOT NULL,
    Rol INTEGER NOT NULL DEFAULT 1, -- 0 'Admin', 1 'Vendedor'
    Estado INTEGER NOT NULL DEFAULT 1 -- 1: Activo, 0: Inactivo
);

-- Tabla de Productos
CREATE TABLE Productos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    PrecioUnitario REAL NOT NULL,
    Estado INTEGER NOT NULL DEFAULT 1 -- 1: Activo, 0: Inactivo
);

-- Tabla de Formas de Pago
CREATE TABLE FormasPago (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,    
    TipoPago TEXT NOT NULL --- Ejemplo: 'Efectivo', 'Tarjeta', 'Transferencia'
);

-- Tabla de Facturas
CREATE TABLE Facturas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    NumeroFactura TEXT NOT NULL UNIQUE,
    ClienteId INTEGER NOT NULL,
    UsuarioId INTEGER NOT NULL,
    Fecha TEXT NOT NULL,
    MontoTotal REAL NOT NULL,
    EstadoPago INTEGER NOT NULL DEFAULT 1, -- 0: PendienteCobro, 1: Pagado, 2: Cancelado
    EstadoFactura INTEGER NOT NULL DEFAULT 1, -- 1: Activo, 0: Inactivo
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id),
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);

-- Tabla de Detalles de Factura
CREATE TABLE DetallesFactura (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FacturaId INTEGER NOT NULL,
    ProductoId INTEGER NOT NULL,
    Cantidad INTEGER NOT NULL,
    PrecioUnitario REAL NOT NULL,
    PrecioTotal REAL NOT NULL,
    FOREIGN KEY (FacturaId) REFERENCES Facturas(Id),
    FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);
-- Tabla de Formas de Pago por factura
CREATE TABLE FormasPagoFactura (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FacturaId INTEGER NOT NULL,
    FormaPagoId INTEGER NOT NULL,
    ValorPagado REAL NOT NULL,
    FOREIGN KEY (FacturaId) REFERENCES Facturas(Id),
    FOREIGN KEY (FormaPagoId) REFERENCES FormasPago(Id)
);
