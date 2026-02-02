# Sistema de Ingreso y Gestión de Facturas

### 🗄️ Modelo de Datos (Resumen)

Entidades principales:

- **Clientes**
- **Usuarios (Vendedores)**
- **Productos**
- **Facturas**
- **DetalleFactura**
- **FormasPago**

Entidades asociativas:

- **FormasPagoFactura**

Relaciones:
- Cliente 1—N Facturas
- Usuario 1—N Facturas
- Factura 1—N DetalleFactura
- Producto 1—N DetalleFactura
- Factura 1—N FormasPagoFactura
- FormasPago 1—N FormasPagoFactura

### --> Las fechas se manejan en formato **ISO-8601** (`yyyy-MM-dd`) para interoperabilidad.
