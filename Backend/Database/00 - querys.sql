/*
    Create DB:
    sqlite3 facturacion.db < schema.sql
*/

/* 1. Insertar usuarios
INSERT INTO Usuarios (Nombre, Rol, PasswordHash, Estado) VALUES ('Dario', 0, '$2a$11$Q.aBLRkhNkq7YDngCnA17eGk.h6pcr0FHfqskppsSS6A3mUxQI3sC', 1);
INSERT INTO Usuarios (Nombre) VALUES ('Tom');
INSERT INTO Usuarios (Nombre) VALUES ('Elena');



-- 2. Insertar clientes
INSERT INTO Clientes (Identificacion, Nombre, Telefono, Correo) VALUES ('123456', 'Arthas Menethil', '555-666', 'arthas@gmail.com');
INSERT INTO Clientes (Identificacion, Nombre, Telefono, Correo) VALUES ('123457', 'Jaina Proudmoore', '555-777', 'jaina@gmail.com');
INSERT INTO Clientes (Identificacion, Nombre, Telefono, Correo) VALUES ('123458', 'Thrall Aslave', '555-888', 'thrall@gmail.com');
INSERT INTO Clientes (Identificacion, Nombre, Telefono, Correo) VALUES ('123459', 'Sylvanas Windrunner', '555-999', 'sylvanas@gmail.com');

-- 3. Insertar Productos
INSERT INTO Productos (Nombre, PrecioUnitario) VALUES ('Laptop', 1200.00);
INSERT INTO Productos (Nombre, PrecioUnitario) VALUES ('Smartphone', 800.00);
INSERT INTO Productos (Nombre, PrecioUnitario) VALUES ('Tablet', 400.00);
INSERT INTO Productos (Nombre, PrecioUnitario) VALUES ('Monitor', 300.00);
INSERT INTO Productos (Nombre, PrecioUnitario) VALUES ('Teclado', 50.00);


-- 4. Insertar una forma de Pago
INSERT INTO FormasPago (TipoPago) VALUES ('Efectivo');
INSERT INTO FormasPago (TipoPago) VALUES ('Tarjeta de Crédito');
INSERT INTO FormasPago (TipoPago) VALUES ('Transferencia Bancaria');


-- 5. Insertar Proveedores
INSERT INTO Proveedores (Identificacion, Nombre, Descripcion, Telefono, Correo) 
    VALUES ('9999999999', 'IBM', 'International Business Machine', '555-999', 'info@ibm.com');
INSERT INTO Proveedores (Identificacion, Nombre, Descripcion, Telefono, Correo) 
    VALUES ('9999999998', 'DELL', 'Proveedora de Laptops', '555-888', 'info@dell.com');
INSERT INTO Proveedores (Identificacion, Nombre, Descripcion, Telefono, Correo) 
    VALUES ('9999999997', 'El Rosado', 'Corporacion El Rosado Ecuador', '555-777', 'info@elrosado.com');
INSERT INTO Proveedores (Identificacion, Nombre, Descripcion, Telefono, Correo) 
    VALUES ('9999999996', 'Tuti', 'Tu Tienda super barato','555-444', 'info@tuti.com');

INSERT INTO ProductoProveedor 
    (ProductoId, ProveedorId, NumeroLote, Precio, Stock) 
VALUES 
    (1, 1, 'IBM-555991', 300, 9);

INSERT INTO ProductoProveedor 
    (ProductoId, ProveedorId, NumeroLote, Precio, Stock) 
VALUES 
    (1, 2, 'DELL-555881', 299, 8);

INSERT INTO ProductoProveedor 
    (ProductoId, ProveedorId, NumeroLote, Precio, Stock) 
VALUES 
    (1, 4, 'TUTI-555441', 290, 4);

INSERT INTO ProductoProveedor 
    (ProductoId, ProveedorId, NumeroLote, Precio, Stock) 
VALUES 
    (6, 3, 'ROS-555771', 300, 9);

INSERT INTO ProductoProveedor 
    (ProductoId, ProveedorId, NumeroLote, Precio, Stock) 
VALUES 
    (6, 4, 'TUTI-555441', 299, 8);

INSERT INTO ProductoProveedor 
    (ProductoId, ProveedorId, NumeroLote, Precio, Stock) 
VALUES 
    (7, 3, 'ROS-555771', 300, 9);

INSERT INTO ProductoProveedor 
    (ProductoId, ProveedorId, NumeroLote, Precio, Stock) 
VALUES 
    (7, 4, 'TUTI-555441', 299, 8);

 

-- drop table if exists ProductoProveedor;


*/

-- 3. Ver si se guardaron
SELECT Id, Identificacion, Nombre, Telefono, Correo, Estado FROM Clientes;
SELECT Id, Identificacion, Nombre, Descripcion, Telefono, Correo, Estado FROM Proveedores;
SELECT Id, Nombre, PasswordHash, Rol, Estado FROM Usuarios;
SELECT Id, Nombre, PrecioUnitario, Estado FROM Productos; 
SELECT Id, TipoPago FROM FormasPago;

SELECT Id, NumeroFactura, ClienteId, UsuarioId, Fecha, MontoTotal, EstadoPago, EstadoFactura FROM Facturas;
SELECT Id, FacturaId, ProductoId, Cantidad, PrecioUnitario, PrecioTotal FROM DetallesFactura;
SELECT Id, FacturaId, FormaPagoId, ValorPagado FROM FormasPagoFactura;

SELECT Id, ProductoId, ProveedorId, NumeroLote, Precio, Stock FROM ProductoProveedor;

SELECT 
    pp.Id AS ProductoProveedorId,
    pp.ProductoId AS ProductoId,
    p.Nombre AS NombreProducto,
    p.Estado,
    p.PrecioUnitario AS PrecioUnitario,
    pp.ProveedorId AS ProveedorId,
    pr.Nombre AS NombreProveedor,
    pp.Precio,
    pp.Stock,
    pp.NumeroLote
FROM ProductoProveedor pp
INNER JOIN Productos p ON pp.ProductoId = p.Id
INNER JOIN Proveedores pr ON pp.ProveedorId = pr.Id;


/*
DROP TABLE IF EXISTS Facturas;

*/

