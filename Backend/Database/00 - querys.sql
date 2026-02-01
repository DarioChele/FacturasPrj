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


-- 3. Insertar una forma de Pago
INSERT INTO FormasPago (TipoPago) VALUES ('Efectivo');
INSERT INTO FormasPago (TipoPago) VALUES ('Tarjeta de Crédito');
INSERT INTO FormasPago (TipoPago) VALUES ('Transferencia Bancaria');

-- drop table if exists Usuarios;


*/

-- 3. Ver si se guardaron
SELECT Id, Identificacion, Nombre, Telefono, Correo, Estado FROM Clientes;
SELECT Id, Nombre, PasswordHash, Rol, Estado FROM Usuarios;
SELECT Id, Nombre, PrecioUnitario, Estado FROM Productos; 
SELECT Id, TipoPago FROM FormasPago;

SELECT Id, NumeroFactura, ClienteId, UsuarioId, Fecha, MontoTotal, EstadoPago, EstadoFactura FROM Facturas;
SELECT Id, FacturaId, ProductoId, Cantidad, PrecioUnitario, PrecioTotal FROM DetallesFactura;
SELECT Id, FacturaId, FormaPagoId, ValorPagado FROM FormasPagoFactura;


/*
DROP TABLE IF EXISTS Facturas;

*/

