using Backend.Models;
using Backend.Persistence.Context;

namespace Backend.Persistence.Repositories;
public class FacturaRepository : IFacturaRepository {
    private readonly ConexionSql _contexto;

    public FacturaRepository(ConexionSql contexto) {
        _contexto = contexto;
    }

    public int Crear(Factura factura) {
        try {
            _contexto.Open();            
            _contexto.InitTran();
            var connection = _contexto.ObtenerConexion();

            // 1. Insertar Cabecera
            var cmdHeader = connection.CreateCommand();
            cmdHeader.CommandText = @"
                INSERT INTO Facturas (NumeroFactura, ClienteId, UsuarioId, Fecha, MontoTotal) 
                VALUES (@num, @cli, @usr, @fecha, @tot);
                SELECT last_insert_rowid();";
            cmdHeader.Parameters.AddWithValue("@num", factura.NumeroFactura);
            cmdHeader.Parameters.AddWithValue("@cli", factura.ClienteId);
            cmdHeader.Parameters.AddWithValue("@usr", factura.UsuarioId);
            cmdHeader.Parameters.AddWithValue("@fecha", factura.Fecha);
            cmdHeader.Parameters.AddWithValue("@tot", factura.MontoTotal);
            
            int facturaId = Convert.ToInt32(cmdHeader.ExecuteScalar());

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
                cmdDetail.ExecuteNonQuery();
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
                    
                    cmdPago.ExecuteNonQuery();

                }
            _contexto.CommitTran(); // Si todo salió bien
            return facturaId;
        }
        catch (Exception) {
            _contexto.RollBack(); // Si algo falló, deshace todo
            throw;
        }
    }

    public Factura? ObtenerPorId(int id)
    {
        throw new NotImplementedException();
    }

    public List<FacturaDTO> ObtenerTodos() {
    var facturaDict = new Dictionary<int, FacturaDTO>();
    try {
        _contexto.Open();
        var connection = _contexto.ObtenerConexion();

        // --- BLOQUE 1: FACTURAS Y DETALLES ---
        using (var command = connection.CreateCommand()) {
            command.CommandText = @"
                SELECT f.Id, f.NumeroFactura, c.Id AS ClienteId, c.Nombre AS NombreCliente, u.Id AS UsuarioId, u.Nombre AS NombreVendedor,
                     f.Fecha, f.MontoTotal, d.Id AS DetalleId, p.Nombre AS NombreProducto, d.Cantidad, d.PrecioUnitario, d.PrecioTotal
                FROM Facturas f
                INNER JOIN Clientes c ON f.ClienteId = c.Id
                INNER JOIN Usuarios u ON f.UsuarioId = u.Id
                LEFT JOIN DetallesFactura d ON f.Id = d.FacturaId
                LEFT JOIN Productos p ON d.ProductoId = p.Id";

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    int facturaId = reader.GetInt32(0);
                    if (!facturaDict.TryGetValue(facturaId, out var factura)) {
                        factura = new FacturaDTO {
                            Id = facturaId,
                            NumeroFactura = reader.GetString(1),
                            ClienteId = reader.GetInt32(2),
                            ClienteNombre = reader.GetString(3),
                            UsuarioId = reader.GetInt32(4),
                            UsuarioNombre = reader.GetString(5),
                            Fecha = reader.GetDateTime(6),
                            MontoTotal = reader.GetDecimal(7),
                            Detalles = new List<DetalleFacturaDTO>(),
                            Pagos = new List<PagoFacturaDTO>() // ¡No olvides inicializarla!
                        };
                        facturaDict.Add(facturaId, factura);
                    }

                    if (!reader.IsDBNull(6)) {
                        factura.Detalles.Add(new DetalleFacturaDTO {
                            Id = reader.GetInt32(6),
                            FacturaId = facturaId,
                            ProductoId = reader.GetInt32(7),
                            ProductoNombre = reader.GetString(8),
                            Cantidad = reader.GetInt32(9),
                            PrecioUnitario = reader.GetDecimal(10),
                            PrecioTotal = reader.GetDecimal(11)
                        });
                    }
                }
            } 
            // --- BLOQUE 2: PAGOS ---
            command.CommandText = @"
                SELECT p.Id, p.FacturaId, p.FormaPagoId, fp.TipoPago, p.ValorPagado
                FROM FormasPagoFactura p
                INNER JOIN FormasPago fp ON p.FormaPagoId = fp.Id";

            using (var reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    int fId = reader.GetInt32(1);
                    if (facturaDict.TryGetValue(fId, out var factura)) {
                        factura.Pagos.Add(new PagoFacturaDTO {
                            Id = reader.GetInt32(0),
                            FacturaId = fId,
                            FormaPagoId = reader.GetInt32(2),
                            FormaPagoNombre = reader.GetString(3),
                            ValorPagado = reader.GetDecimal(4)                            
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
}