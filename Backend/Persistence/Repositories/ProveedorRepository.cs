using Backend.Models;
using Backend.Persistence.Context;
using Backend.Persistence.Repositories.Interfaces;

namespace Backend.Persistence.Repositories;
public class ProveedorRepository:IProveedorRepository{
    private readonly ConexionSql _contexto;

    public ProveedorRepository(ConexionSql contexto){
        _contexto = contexto;
    }

    public async Task<List<ProveedorDTO>> ObtenerTodos( string? estado = null, string? identificacion = null){
        var proveedors = new List<Proveedor>();
        try{
            await _contexto.OpenAsync();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"SELECT Id, Identificacion, Nombre, Descripcion, Telefono, Correo, Estado FROM Proveedores";
            if (estado != null) {
                command.CommandText += " WHERE Estado = @estado";
                command.Parameters.AddWithValue("@estado", estado);
            }
            if (identificacion != null) {
                command.CommandText += " AND Identificacion = @ident";
                command.Parameters.AddWithValue("@ident", identificacion);
            }
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()){
                proveedors.Add(new Proveedor {                    
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Identificacion = reader.GetString(reader.GetOrdinal("Identificacion")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                    Correo = reader.IsDBNull(reader.GetOrdinal("Correo")) ? null : reader.GetString(reader.GetOrdinal("Correo")),
                    Estado = reader.GetInt32(reader.GetOrdinal("Estado"))
                });
            }
        }
        finally { _contexto.Close(); }
        // Transformamos la lista de entidades a DTOs

        return proveedors.Select(c => new ProveedorDTO {
            Id = c.Id,
            Identificacion = c.Identificacion,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion,
            Telefono = c.Telefono,
            Correo = c.Correo,
            Estado = c.Estado
        }).ToList();
        
    }
    public async Task<int> Crear(Proveedor proveedor){
        try{
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                INSERT INTO Proveedores (Identificacion, Nombre, Descripcion, Telefono, Correo) 
                VALUES (@ident, @nom, @desc, @tel, @corr);
                SELECT last_insert_rowid();";
            
            command.Parameters.AddWithValue("@ident", proveedor.Identificacion);
            command.Parameters.AddWithValue("@nom", proveedor.Nombre);
            command.Parameters.AddWithValue("@desc", proveedor.Descripcion);
            command.Parameters.AddWithValue("@tel", (object?)proveedor.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@corr", (object?)proveedor.Correo ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        finally { _contexto.Close(); }
    }
    public async Task<bool> Modificar(Proveedor proveedor){
        try{
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                UPDATE Proveedores 
                SET Nombre = @nom, Descripcion = @desc, Telefono = @tel, Correo = @corr
                WHERE Id = @id AND Identificacion = @ident ";

            command.Parameters.AddWithValue("@id", proveedor.Id);
            command.Parameters.AddWithValue("@ident", proveedor.Identificacion);
            command.Parameters.AddWithValue("@nom", proveedor.Nombre);
            command.Parameters.AddWithValue("@desc", proveedor.Descripcion);
            command.Parameters.AddWithValue("@tel", (object?)proveedor.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@corr", (object?)proveedor.Correo ?? DBNull.Value);

            int filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }
        finally { _contexto.Close(); }
    }
    public async Task<bool> Eliminar(Proveedor proveedor){
        try{
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                UPDATE Proveedores
                SET estado = 0
                WHERE Id = @id ";
            command.Parameters.AddWithValue("@id", proveedor.Id);

            int filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }
        finally { _contexto.Close(); }
    }

    
}
