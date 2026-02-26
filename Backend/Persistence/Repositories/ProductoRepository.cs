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

    public async Task<List<ProductoDTO>> ObtenerTodos(string? estado = null, string? id = null){
        var productosDict = new Dictionary<int, ProductoDTO>();
        try{
            await _contexto.OpenAsync();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                SELECT 
                    p.Id AS ProductoId,
                    pr.Id AS ProductoProveedorId,                    
                    p.Nombre AS NombreProducto,
                    p.Estado,
                    p.PrecioUnitario AS PrecioUnitario,
                    pp.ProveedorId AS ProveedorId,
                    pr.Nombre AS NombreProveedor,
                    pp.Precio,
                    pp.Stock,
                    pp.NumeroLote
                FROM Productos p
                LEFT JOIN ProductoProveedor pp ON pp.ProductoId = p.Id
                LEFT JOIN Proveedores pr ON pp.ProveedorId = pr.Id
                WHERE ";            
            if (estado != null) {
                command.CommandText += " p.Estado = @estado and ";
                command.Parameters.AddWithValue("@estado", estado);
            }
            if (id != null) {
                command.CommandText += " p.Id = @Id and ";
                command.Parameters.AddWithValue("@Id", id);
            }
            command.CommandText += " 1 = 1; ";
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
                if (!reader.IsDBNull(reader.GetOrdinal("ProveedorId"))){
                    var detalle = new DetalleProveedorDTO { 
                        //ProveedorId = reader.GetInt32(reader.GetOrdinal("ProveedorId")),
                        ProveedorId = reader.IsDBNull(reader.GetOrdinal("ProveedorId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProveedorId")), 
                        NombreProveedor = reader.IsDBNull(reader.GetOrdinal("NombreProveedor")) ? null : reader.GetString(reader.GetOrdinal("NombreProveedor")),                        
                        Precio = reader.IsDBNull(reader.GetOrdinal("Precio")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Precio")), 
                        Stock = reader.IsDBNull(reader.GetOrdinal("Stock")) ? 0 : reader.GetInt32(reader.GetOrdinal("Stock")),
                        NumeroLote = reader.IsDBNull(reader.GetOrdinal("NumeroLote")) ? null : reader.GetString(reader.GetOrdinal("NumeroLote")),
                    }; 
                    productosDict[productoId].Proveedores.Add(detalle); 
                    // 🔑 Aquí actualizamos el stock total acumulando 
                    productosDict[productoId].StockTotal += (int)detalle.Stock;
                }
                
            }
        } catch (Exception ex){
            Console.WriteLine(ex.Message);
        } finally { _contexto.Close(); }

        return productosDict.Values.ToList();
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
        catch (Exception) {            
            await _contexto.RollBackAsync(); // Si algo falló, deshace todo
            throw;
        }
    }   

    public async Task<bool> Modificar(Producto producto){
        try{
            await _contexto.OpenAsync();
            await _contexto.InitTranAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                UPDATE Productos 
                SET Nombre = @nom, PrecioUnitario = @precio, Estado = @estado
                WHERE Id = @id";
            command.Parameters.AddWithValue("@id", producto.Id);
            command.Parameters.AddWithValue("@nom", producto.Nombre);
            command.Parameters.AddWithValue("@precio", producto.PrecioUnitario);
            command.Parameters.AddWithValue("@estado", producto.Estado);

            int filasInserted = await command.ExecuteNonQueryAsync();

            using var comdDel = _contexto.ObtenerConexion().CreateCommand();
            comdDel.CommandText = @"
                DELETE 
                FROM ProductoProveedor
                WHERE ProductoId = @productoId";
            comdDel.Parameters.AddWithValue("@productoId", producto.Id);

            int filasDeleted = await comdDel.ExecuteNonQueryAsync();

            foreach (var proveedor in producto.Proveedores) {
                var cmdDetail = _contexto.ObtenerConexion().CreateCommand();
                cmdDetail.CommandText = @"
                    INSERT INTO ProductoProveedor (ProductoId, ProveedorId, NumeroLote, Precio, Stock) 
                    VALUES (@productoId, @proveedorId, @numeroLote, @precio, @stock);";
                cmdDetail.Parameters.AddWithValue("@productoId", producto.Id);
                cmdDetail.Parameters.AddWithValue("@proveedorId", proveedor.ProveedorId);
                cmdDetail.Parameters.AddWithValue("@numeroLote", proveedor.NumeroLote);
                cmdDetail.Parameters.AddWithValue("@precio", proveedor.Precio);
                cmdDetail.Parameters.AddWithValue("@stock", proveedor.Stock);
                await cmdDetail.ExecuteNonQueryAsync();
            }
            await _contexto.CommitTranAsync();
            return filasInserted > 0;
        }catch (Exception ex) {
            Console.WriteLine("Error en Actualizar Prodcuto  ->>   " +  ex.Message);
            await _contexto.RollBackAsync(); // Si algo falló, deshace todo
            throw;
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
