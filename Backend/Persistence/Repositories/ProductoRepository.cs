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
        var productos = new List<Producto>();
        try{
            await _contexto.OpenAsync();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = "SELECT Id, Nombre, PrecioUnitario, Estado FROM Productos";
            if (estado != null) {
                command.CommandText += " WHERE Estado = @estado";
                command.Parameters.AddWithValue("@estado", estado);
            }
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()){
                productos.Add(new Producto {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                    Estado = reader.GetInt32(reader.GetOrdinal("Estado"))
                });
            }
        } finally { _contexto.Close(); }

        return productos.Select(p => new ProductoDTO {
            Id = p.Id,
            Nombre = p.Nombre,
            PrecioUnitario = p.PrecioUnitario,
            Estado = p.Estado
        }).ToList();
    }

    public async Task<int> Crear(Producto producto){
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
