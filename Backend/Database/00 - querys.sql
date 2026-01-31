-- 1. Insertar un vendedor
INSERT INTO Usuarios (Nombre, Rol) VALUES ('Dario', 'Vendedor');

-- 2. Insertar un cliente
INSERT INTO Clientes (Identificacion, Nombre, Telefono, Correo) 
VALUES ('123456', 'Arthas Menethil', '555-666', 'arthas@gmail.com');

-- 3. Ver si se guardaron
SELECT Id, Identificacion, Nombre, Telefono, Correo FROM Clientes;