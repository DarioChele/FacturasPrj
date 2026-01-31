-- Tabla de Clientes
CREATE TABLE Clientes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Identificacion TEXT NOT NULL,
    Nombre TEXT NOT NULL,
    Telefono TEXT,
    Correo TEXT
);

-- Tabla de Usuarios/Vendedores
CREATE TABLE Usuarios (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    Rol TEXT NOT NULL -- Ejemplo: 'Vendedor', 'Admin'
);

-- Tabla de Productos
CREATE TABLE Productos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    PrecioUnitario REAL NOT NULL
);

-- Tabla de Facturas
CREATE TABLE Facturas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    NumeroFactura TEXT NOT NULL UNIQUE,
    ClienteId INTEGER NOT NULL,
    UsuarioId INTEGER NOT NULL,
    Fecha TEXT NOT NULL,
    MontoTotal REAL NOT NULL,
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

-- Tabla de Formas de Pago
CREATE TABLE FormasPago (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FacturaId INTEGER NOT NULL,
    TipoPago TEXT NOT NULL, -- Ejemplo: 'Efectivo', 'Tarjeta', 'Transferencia'    
    FOREIGN KEY (FacturaId) REFERENCES Facturas(Id)
);