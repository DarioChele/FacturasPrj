/*
    Create DB:
    sqlite3 facturacion.db < schema.sql
*/

/* 1. Insertar un vendedor

INSERT INTO Usuarios (Nombre, Rol) VALUES ('Dario', 'Vendedor');

-- 2. Insertar un cliente
INSERT INTO Clientes (Identificacion, Nombre, Telefono, Correo) 
VALUES ('123456', 'Arthas Menethil', '555-666', 'arthas@gmail.com');

-- 3. Insertar una forma de Pago
INSERT INTO FormasPago (TipoPago) VALUES ('Efectivo');
INSERT INTO FormasPago (TipoPago) VALUES ('Tarjeta de Crédito');
INSERT INTO FormasPago (TipoPago) VALUES ('Transferencia Bancaria');


*/

-- 3. Ver si se guardaron
SELECT Id, Identificacion, Nombre, Telefono, Correo FROM Clientes;
SELECT Id, Nombre, Rol FROM Usuarios;
SELECT Id, Nombre, PrecioUnitario FROM Productos; 
SELECT Id, TipoPago FROM FormasPago; 
SELECT Id, NumeroFactura, ClienteId, UsuarioId, Fecha, MontoTotal FROM Facturas;
SELECT Id, FacturaId, ProductoId, Cantidad, PrecioUnitario, PrecioTotal FROM DetallesFactura;
SELECT Id, FacturaId, FormaPagoId, ValorPagado FROM FormasPagoFactura;


/*
DROP TABLE IF EXISTS Facturas;

*/

