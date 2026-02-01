using Backend.Models;
using Backend.Persistence.Context;
using Backend.Persistence.Repositories.Interfaces;
namespace Backend.Persistence.Repositories;
public class FacturaRepository : IFacturaRepository {
    private readonly ConexionSql _contexto;    

    public FacturaRepository(ConexionSql contexto) {
        _contexto = contexto;
    }

    public async Task<int> Crear(Factura factura) {
        try {
            await _contexto.OpenAsync();            
            await _contexto.InitTranAsync();
            var connection = _contexto.ObtenerConexion();

            // 1. Insertar Cabecera
            var cmdHeader = connection.CreateCommand();
            cmdHeader.CommandText = @"
                INSERT INTO Facturas (NumeroFactura, ClienteId, UsuarioId, Fecha, MontoTotal, EstadoPago) 
                VALUES (@num, @cli, @usr, @fecha, @tot, @estadoPago);
                SELECT last_insert_rowid();";
            cmdHeader.Parameters.AddWithValue("@num", factura.NumeroFactura);
            cmdHeader.Parameters.AddWithValue("@cli", factura.ClienteId);
            cmdHeader.Parameters.AddWithValue("@usr", factura.UsuarioId);
            cmdHeader.Parameters.AddWithValue("@fecha", factura.Fecha);
            cmdHeader.Parameters.AddWithValue("@tot", factura.MontoTotal);
            cmdHeader.Parameters.AddWithValue("@estadoPago", factura.EstadoPago);
            
            int facturaId = Convert.ToInt32(await cmdHeader.ExecuteScalarAsync());

            // 2. Insertar Detalles
            foreach (var detalle in factura.Detalles) {
                var cmdDetail = connection.CreateCommand();
                cmdDetail.CommandText = @"
                    INSERT INTO DetallesFactura (FacturaId, ProductoId, Cantidad, PrecioUnitario, PrecioTotal)
                    VALUES (@fId, @pId, @cant, @pre, @ptot);";
                cmdDetail.Parameters.AddWithValue("@fId", facturaId);
                cmdDetail.Parameters.AddWithValue("@pId", detalle.ProductoId);
                cmdDetail.Parameters.AddWithValue("@cant", detalle.Cantidad);
                cmdDetail.Parameters.AddWithValue("@pre", detalle.PrecioUnitario);
                cmdDetail.Parameters.AddWithValue("@ptot", detalle.PrecioTotal);
                await cmdDetail.ExecuteNonQueryAsync();
            }
            // 3. Insertar Formas de pago
            foreach (var pago in factura.Pagos) {
                    var cmdPago = connection.CreateCommand();
                    cmdPago.CommandText = @"
                        INSERT INTO FormasPagoFactura (FacturaId, FormaPagoId, ValorPagado)
                        VALUES (@fId, @pId, @val);";
                    
                    cmdPago.Parameters.AddWithValue("@fId", facturaId);
                    cmdPago.Parameters.AddWithValue("@pId", pago.FormaPagoId);
                    cmdPago.Parameters.AddWithValue("@val", pago.ValorPagado);
                    
                    await cmdPago.ExecuteNonQueryAsync();

                }
            await _contexto.CommitTranAsync(); // Si todo salió bien
            return facturaId;
        }
        catch (Exception) {
            await _contexto.RollBackAsync(); // Si algo falló, deshace todo
            throw;
        }
    }   

    //public List<FacturaDTO> ObtenerTodos() {
    public async Task<List<FacturaDTO>> ObtenerTodos(string? numero = null, DateTime? fecha = null, decimal? monto = null){
    var facturaDict = new Dictionary<int, FacturaDTO>();
    try {
        await _contexto.OpenAsync();
        var connection = _contexto.ObtenerConexion();

        // --- BLOQUE 1: FACTURAS Y DETALLES ---
        
            string sql = @"
                SELECT  f.Id, f.NumeroFactura, c.Id AS ClienteId, c.Identificacion AS IdentificacionCliente, c.Nombre AS NombreCliente, 
                        u.Id AS UsuarioId, u.Nombre AS NombreVendedor, f.Fecha, f.MontoTotal, f.EstadoPago, f.EstadoFactura, d.ProductoId AS ProductoId, 
                        p.Nombre AS NombreProducto, d.Cantidad, d.PrecioUnitario, d.PrecioTotal
                FROM Facturas f
                    INNER JOIN Clientes c ON f.ClienteId = c.Id
                    INNER JOIN Usuarios u ON f.UsuarioId = u.Id
                    LEFT JOIN DetallesFactura d ON f.Id = d.FacturaId
                    LEFT JOIN Productos p ON d.ProductoId = p.Id
                WHERE 1=1 ";
        using (var command = connection.CreateCommand()) {
            // Construcción dinámica del SQL
            if (!string.IsNullOrEmpty(numero)) sql += " AND f.NumeroFactura LIKE @num";
            if (fecha.HasValue) sql += " AND date(f.Fecha) = date(@fec)";
            if (monto.HasValue) sql += " AND f.MontoTotal >= @mon";

            command.CommandText = sql;

            // Añadir parámetros de forma agnóstica al proveedor
            if (!string.IsNullOrEmpty(numero)) command.Parameters.AddWithValue("@num", $"%{numero}%");
            if (fecha.HasValue) command.Parameters.AddWithValue("@fec", fecha.Value.ToString("yyyy-MM-dd"));
            if (monto.HasValue) command.Parameters.AddWithValue("@mon", monto.Value);

            using (var reader = await command.ExecuteReaderAsync()) {
                while (await reader.ReadAsync()) {
                    int facturaId = reader.GetInt32(reader.GetOrdinal("Id"));
                    if (!facturaDict.TryGetValue(facturaId, out var factura)) {
                        factura = new FacturaDTO {
                            Id = facturaId,
                            NumeroFactura = reader.GetString(reader.GetOrdinal("NumeroFactura")),
                            ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                            ClienteIdentificacion = reader.GetString(reader.GetOrdinal("IdentificacionCliente")),
                            ClienteNombre = reader.GetString(reader.GetOrdinal("NombreCliente")),
                            UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                            UsuarioNombre = reader.GetString(reader.GetOrdinal("NombreVendedor")),
                            Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                            MontoTotal = reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                            EstadoPago = reader.GetInt32(reader.GetOrdinal("EstadoPago")),
                            ///--- Colocar aquí los nuevos campos ---
                            EstadoFactura = reader.GetInt32(reader.GetOrdinal("EstadoFactura")),
                            Detalles = new List<DetalleFacturaDTO>(),
                            Pagos = new List<PagoFacturaDTO>() 
                        };
                        facturaDict.Add(facturaId, factura);
                    }
                    if (!reader.IsDBNull(reader.GetOrdinal("ProductoId"))) {
                        factura.Detalles.Add(new DetalleFacturaDTO {
                            FacturaId = facturaId,
                            ProductoId = reader.GetInt32(reader.GetOrdinal("ProductoId")),
                            ProductoNombre = reader.GetString(reader.GetOrdinal("NombreProducto")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                            PrecioTotal = reader.GetDecimal(reader.GetOrdinal("PrecioTotal"))
                        });
                    }
                }
            }
            // --- BLOQUE 2: PAGOS ---
            command.Parameters.Clear();
            string sqlPagos = @"
                SELECT p.Id, p.FacturaId, p.FormaPagoId, fp.TipoPago, p.ValorPagado
                FROM FormasPagoFactura p
                INNER JOIN FormasPago fp ON p.FormaPagoId = fp.Id
                INNER JOIN Facturas f ON p.FacturaId = f.Id
                WHERE 1=1 ";

            // REPETIMOS la lógica de filtros del Bloque 1
            if (!string.IsNullOrEmpty(numero)) {
                sqlPagos += " AND f.NumeroFactura LIKE @num";
                command.Parameters.AddWithValue("@num", $"%{numero}%");
            }
            if (fecha.HasValue) {
                sqlPagos += " AND date(f.Fecha) = date(@fec)";
                command.Parameters.AddWithValue("@fec", fecha.Value.ToString("yyyy-MM-dd"));
            }
            if (monto.HasValue) {
                sqlPagos += " AND f.MontoTotal >= @mon";
                command.Parameters.AddWithValue("@mon", monto.Value);
            }

            command.CommandText = sqlPagos;
                using (var readerPagos = await command.ExecuteReaderAsync()) {
                    while (await readerPagos.ReadAsync()) {
                        int fId = readerPagos.GetInt32(readerPagos.GetOrdinal("FacturaId"));
                        if (facturaDict.TryGetValue(fId, out var factura)){
                            factura.Pagos.Add(new PagoFacturaDTO {
                                Id = readerPagos.GetInt32(readerPagos.GetOrdinal("Id")),
                                FacturaId = fId,
                                FormaPagoId = readerPagos.GetInt32(readerPagos.GetOrdinal("FormaPagoId")),
                                FormaPagoNombre = readerPagos.GetString(readerPagos.GetOrdinal("TipoPago")),
                                ValorPagado = readerPagos.GetDecimal(readerPagos.GetOrdinal("ValorPagado"))                            
                            });    
                        }
                        
                    }
                }
        }
    } finally {
        _contexto.Close();
    }
        return facturaDict.Values.ToList();
    }
    public async Task<FacturaDTO?> ObtenerPorId(int id){
        FacturaDTO? factura = null;
        try{
            await _contexto.OpenAsync();
            var connection = _contexto.ObtenerConexion();

            using (var command = connection.CreateCommand()){
                // --- BLOQUE 1: CABECERA Y DETALLES ---
                command.CommandText = @"
                    SELECT f.Id, f.NumeroFactura, c.Id AS ClienteId, c.Identificacion AS IdentificacionCliente, 
                        c.Nombre AS NombreCliente, u.Id AS UsuarioId, u.Nombre AS NombreVendedor, 
                        f.Fecha, f.MontoTotal, f.EstadoPago, f.EstadoFactura, 
                        d.ProductoId, p.Nombre AS NombreProducto, d.Cantidad, d.PrecioUnitario, d.PrecioTotal
                    FROM Facturas f
                    INNER JOIN Clientes c ON f.ClienteId = c.Id
                    INNER JOIN Usuarios u ON f.UsuarioId = u.Id
                    LEFT JOIN DetallesFactura d ON f.Id = d.Id 
                    LEFT JOIN Productos p ON d.ProductoId = p.Id
                    WHERE f.Id = @id";

                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync()){
                    while (await reader.ReadAsync()){
                        if (factura == null){
                            factura = new FacturaDTO{
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                NumeroFactura = reader.GetString(reader.GetOrdinal("NumeroFactura")),
                                ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                                ClienteIdentificacion = reader.GetString(reader.GetOrdinal("IdentificacionCliente")),
                                ClienteNombre = reader.GetString(reader.GetOrdinal("NombreCliente")),
                                UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                                UsuarioNombre = reader.GetString(reader.GetOrdinal("NombreVendedor")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                                MontoTotal = reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                                EstadoPago = reader.GetInt32(reader.GetOrdinal("EstadoPago")),
                                EstadoFactura = reader.GetInt32(reader.GetOrdinal("EstadoFactura"))
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ProductoId"))){
                            factura.Detalles.Add(new DetalleFacturaDTO{
                                FacturaId = id,
                                ProductoId = reader.GetInt32(reader.GetOrdinal("ProductoId")),
                                ProductoNombre = reader.GetString(reader.GetOrdinal("NombreProducto")),
                                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                                PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                                PrecioTotal = reader.GetDecimal(reader.GetOrdinal("PrecioTotal"))
                            });
                        }
                    }
                }

                // --- BLOQUE 2: PAGOS (Solo si la factura existe) ---
                if (factura != null){
                    command.Parameters.Clear();
                    command.CommandText = @"
                        SELECT p.Id, p.FacturaId, p.FormaPagoId, fp.TipoPago, p.ValorPagado
                        FROM FormasPagoFactura p
                        INNER JOIN FormasPago fp ON p.FormaPagoId = fp.Id
                        WHERE p.FacturaId = @id";
                    command.Parameters.AddWithValue("@id", id);

                    using (var readerPagos = await command.ExecuteReaderAsync()){
                        while (await readerPagos.ReadAsync()){
                            factura.Pagos.Add(new PagoFacturaDTO{
                                Id = readerPagos.GetInt32(readerPagos.GetOrdinal("Id")),
                                //FacturaId = readerPagos.GetInt32(readerPagos.GetOrdinal("FacturaId")),
                                FacturaId = id,
                                FormaPagoId = readerPagos.GetInt32(readerPagos.GetOrdinal("FormaPagoId")),
                                FormaPagoNombre = readerPagos.GetString(readerPagos.GetOrdinal("TipoPago")),
                                ValorPagado = readerPagos.GetDecimal(readerPagos.GetOrdinal("ValorPagado"))
                            });
                        }
                    }
                }
            }
        }
        finally { _contexto.Close(); }
        return factura;
    }
}