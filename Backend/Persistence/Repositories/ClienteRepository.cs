using Backend.Models;
using Backend.Persistence.Context;
using Backend.Persistence.Repositories.Interfaces;

namespace Backend.Persistence.Repositories;
public class ClienteRepository:IClienteRepository{
    private readonly ConexionSql _contexto;

    public ClienteRepository(ConexionSql contexto){
        _contexto = contexto;
    }

    public async Task<List<ClienteDTO>> ObtenerTodos(){
        var clientes = new List<Cliente>();
        try{
            await _contexto.OpenAsync();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = "SELECT Id, Identificacion, Nombre, Telefono, Correo, Estado FROM Clientes";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()){
                clientes.Add(new Cliente {                    
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Identificacion = reader.GetString(reader.GetOrdinal("Identificacion")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                    Correo = reader.IsDBNull(reader.GetOrdinal("Correo")) ? null : reader.GetString(reader.GetOrdinal("Correo")),
                    Estado = reader.GetInt32(reader.GetOrdinal("Estado"))
                });
            }
        }
        finally { _contexto.Close(); }
        // Transformamos la lista de entidades a DTOs

        return clientes.Select(c => new ClienteDTO {
            Id = c.Id,
            Identificacion = c.Identificacion,
            Nombre = c.Nombre,
            Telefono = c.Telefono,
            Correo = c.Correo,
            Estado = c.Estado
        }).ToList();
        
    }


    public async Task<ClienteDTO?> ObtenerPorIdentificacion(string identificacion){
        var cliente = new ClienteDTO();
        try{
            await _contexto.OpenAsync();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = "SELECT Id, Identificacion, Nombre, Telefono, Correo, Estado FROM Clientes WHERE Identificacion = @ident";
            command.Parameters.AddWithValue("@ident", identificacion);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()){
                cliente = new ClienteDTO {                    
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Identificacion = reader.GetString(reader.GetOrdinal("Identificacion")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                    Correo = reader.IsDBNull(reader.GetOrdinal("Correo")) ? null : reader.GetString(reader.GetOrdinal("Correo")),
                    Estado = reader.GetInt32(reader.GetOrdinal("Estado"))
                };
            }
        }
        finally { _contexto.Close(); }
        return cliente;
    }

    public async Task<int> Crear(Cliente cliente){
        try{
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                INSERT INTO Clientes (Identificacion, Nombre, Telefono, Correo) 
                VALUES (@ident, @nom, @tel, @corr);
                SELECT last_insert_rowid();";
            
            command.Parameters.AddWithValue("@ident", cliente.Identificacion);
            command.Parameters.AddWithValue("@nom", cliente.Nombre);
            command.Parameters.AddWithValue("@tel", (object)cliente.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@corr", (object)cliente.Correo ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        finally { _contexto.Close(); }
    }
    public async Task<bool> Modificar(Cliente cliente){
        try{
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                UPDATE Clientes 
                SET Nombre = @nom, Telefono = @tel, Correo = @corr
                WHERE Id = @id AND Identificacion = @ident ";

            command.Parameters.AddWithValue("@id", cliente.Id);
            command.Parameters.AddWithValue("@ident", cliente.Identificacion);
            command.Parameters.AddWithValue("@nom", cliente.Nombre);
            command.Parameters.AddWithValue("@tel", (object?)cliente.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@corr", (object?)cliente.Correo ?? DBNull.Value);

            int filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }
        finally { _contexto.Close(); }
    }
    public async Task<bool> Eliminar(Cliente cliente){
        try{
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                UPDATE Clientes 
                SET estado = 0
                WHERE Id = @id ";
            command.Parameters.AddWithValue("@id", cliente.Id);

            int filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }
        finally { _contexto.Close(); }
    }

    
}
