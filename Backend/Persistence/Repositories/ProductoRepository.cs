using Microsoft.Data.Sqlite;
using Backend.Models;
using Backend.Persistence.Context;
using Backend.Persistence.Repositories.Interfaces;

namespace Backend.Persistence.Repositories;
public class ProductoRepository:IProductoRepository{
    private readonly ConexionSql _contexto;

    public ProductoRepository(ConexionSql contexto){
    _contexto = contexto;
    }

    public async Task<List<ProductoDTO>> ObtenerTodos(string? estado = null){
        var productosDict = new Dictionary<int, ProductoDTO>();
        try{
            await _contexto.OpenAsync();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
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
                INNER JOIN Proveedores pr ON pp.ProveedorId = pr.Id";
            if (estado != null) {
                command.CommandText += " WHERE p.Estado = @estado";
                command.Parameters.AddWithValue("@estado", estado);
            }
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()){
                int productoId = reader.GetInt32(reader.GetOrdinal("ProductoId")); 
                if (!productosDict.ContainsKey(productoId)) { 
                    productosDict[productoId] = new ProductoDTO { 
                        Id = productoId, 
                        Nombre = reader.GetString(reader.GetOrdinal("NombreProducto")),
                        PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                        Estado = reader.GetInt32(reader.GetOrdinal("Estado")), 
                        StockTotal = 0, 
                        Proveedores = new List<DetalleProveedorDTO>() }; 
                }
                var detalle = new DetalleProveedorDTO { 
                    ProveedorId = reader.GetInt32(reader.GetOrdinal("ProveedorId")), 
                    NombreProveedor = reader.GetString(reader.GetOrdinal("NombreProveedor")), 
                    Precio = reader.GetDecimal(reader.GetOrdinal("Precio")), 
                    Stock = reader.GetInt32(reader.GetOrdinal("Stock")), 
                    NumeroLote = reader.GetString(reader.GetOrdinal("NumeroLote")) 
                }; 
                productosDict[productoId].Proveedores.Add(detalle); 
                // 🔑 Aquí actualizamos el stock total acumulando 
                productosDict[productoId].StockTotal += detalle.Stock;
            }
        } finally { _contexto.Close(); }

        return productosDict.Values.ToList();
    }

    public async Task<int> Crear_(Producto producto){
        try{
            await _contexto.OpenAsync();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                INSERT INTO Productos (Nombre, PrecioUnitario) 
                VALUES (@nom, @precio);
                SELECT last_insert_rowid();";
            
            command.Parameters.AddWithValue("@nom", producto.Nombre);
            command.Parameters.AddWithValue("@precio", producto.PrecioUnitario);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }finally { _contexto.Close(); }
    }
    public async Task<int> Crear(Producto producto) {
        try {
            await _contexto.OpenAsync();            
            await _contexto.InitTranAsync();
            var connection = _contexto.ObtenerConexion();

            // 1. Insertar Cabecera
            var cmdHeader = connection.CreateCommand();
            cmdHeader.CommandText = @"
                INSERT INTO Productos (Nombre, PrecioUnitario) 
                VALUES (@nom, @precio);
                SELECT last_insert_rowid();";
            cmdHeader.Parameters.AddWithValue("@nom", producto.Nombre);
            cmdHeader.Parameters.AddWithValue("@precio", producto.PrecioUnitario);            
            
            int productoId = Convert.ToInt32(await cmdHeader.ExecuteScalarAsync());

            // 2. Insertar Detalles
            foreach (var proveedor in producto.Proveedores) {
                var cmdDetail = connection.CreateCommand();
                cmdDetail.CommandText = @"
                    INSERT INTO ProductoProveedor (ProductoId, ProveedorId, NumeroLote, Precio, Stock) 
                    VALUES (@productoId, @proveedorId, @numeroLote, @precio, @stock);";
                cmdDetail.Parameters.AddWithValue("@productoId", productoId);
                cmdDetail.Parameters.AddWithValue("@proveedorId", proveedor.ProveedorId);
                cmdDetail.Parameters.AddWithValue("@numeroLote", proveedor.NumeroLote);
                cmdDetail.Parameters.AddWithValue("@precio", proveedor.Precio);
                cmdDetail.Parameters.AddWithValue("@stock", proveedor.Stock);
                await cmdDetail.ExecuteNonQueryAsync();
            }
            await _contexto.CommitTranAsync(); // Si todo salió bien
            return productoId;
        }
        catch (Exception ex) {
            
            await _contexto.RollBackAsync(); // Si algo falló, deshace todo
            throw;
        }
    }   







































    public async Task<bool> Modificar(Producto producto){
        try{
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                UPDATE Productos 
                SET Nombre = @nom, PrecioUnitario = @precio, Estado = @estado
                WHERE Id = @id";
            command.Parameters.AddWithValue("@id", producto.Id);
            command.Parameters.AddWithValue("@nom", producto.Nombre);
            command.Parameters.AddWithValue("@precio", producto.PrecioUnitario);
            command.Parameters.AddWithValue("@estado", producto.Estado);

            int filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }
        finally { _contexto.Close(); }
    }
    public async Task<bool> Eliminar(Producto producto){
        try{
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                UPDATE Productos 
                SET estado = 0
                WHERE Id = @id ";
            command.Parameters.AddWithValue("@id", producto.Id);

            int filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }
        finally { _contexto.Close(); }
    }
}
