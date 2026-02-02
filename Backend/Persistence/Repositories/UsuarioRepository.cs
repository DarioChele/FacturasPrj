using Microsoft.Data.Sqlite;
using Backend.Models;
using Backend.Persistence.Context;

namespace Backend.Persistence.Repositories;
    public class UsuarioRepository:IUsuarioRepository{
        private readonly ConexionSql _contexto;

        public UsuarioRepository(ConexionSql contexto){
        _contexto = contexto;
    }
    public async Task<List<UsuarioDTO>> ObtenerTodos(){
        var usuarios = new List<Usuario>();
        try{
            await _contexto.OpenAsync();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = "SELECT Id, Nombre, Rol, Estado FROM Usuarios";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()){
                usuarios.Add(new Usuario {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Rol = reader.GetInt32(reader.GetOrdinal("Rol")),
                    Estado = reader.GetInt32(reader.GetOrdinal("Estado"))
                });
            }
        }finally { _contexto.Close(); }
        
        return usuarios.Select(u => new UsuarioDTO {
                Id = u.Id,
                Nombre = u.Nombre,
                Rol = u.Rol,
                Estado = u.Estado
        }).ToList();
    }
    public async Task<UsuarioDTO?> ObtenerPorId(int id){
        try {
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = "SELECT Id, Nombre, Rol, Estado FROM Usuarios WHERE Id = @id AND Estado = 1";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                return new UsuarioDTO {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Rol = reader.GetInt32(reader.GetOrdinal("Rol")),
                    Estado = reader.GetInt32(reader.GetOrdinal("Estado"))
                };
            }
            return null;
        }
        finally { _contexto.Close(); }     
    }
    public async Task<Usuario?> ObtenerPorNombre(string nombre) {
        try {
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = "SELECT Id, Nombre, Rol, PasswordHash, Estado FROM Usuarios WHERE Nombre = @nombre AND Estado = 1";
            command.Parameters.AddWithValue("@nombre", nombre);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                return new Usuario {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Rol = reader.GetInt32(reader.GetOrdinal("Rol")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    Estado = reader.GetInt32(reader.GetOrdinal("Estado"))
                };
            }
            return null;
        }
        finally { _contexto.Close(); }
    }
    public async Task<int> Crear(Usuario usuario){
        try{
            await _contexto.OpenAsync();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
            INSERT INTO Usuarios (Nombre, Rol, PasswordHash, Estado) 
            VALUES (@nom, @rol, @hash, @est);
            SELECT last_insert_rowid();";
            
            command.Parameters.AddWithValue("@nom", usuario.Nombre);
            command.Parameters.AddWithValue("@rol", usuario.Rol);
            command.Parameters.AddWithValue("@hash", usuario.PasswordHash);
            command.Parameters.AddWithValue("@est", 1); // Activo por defecto
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        finally { _contexto.Close(); }
    }
    public async Task<bool> Modificar(Usuario usuario){
        try{
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                UPDATE Usuarios 
                SET Nombre = @nom, Rol = @rol, PasswordHash = @hash, Estado = @est
                WHERE Id = @id";
            command.Parameters.AddWithValue("@id", usuario.Id);
            command.Parameters.AddWithValue("@nom", usuario.Nombre);
            command.Parameters.AddWithValue("@rol", usuario.Rol);
            command.Parameters.AddWithValue("@hash", usuario.PasswordHash);
            command.Parameters.AddWithValue("@est", usuario.Estado);

            int filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }
        finally { _contexto.Close(); }
    }
    public async Task<bool> Eliminar(Usuario usuario){
        try{
            await _contexto.OpenAsync();
            using var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                UPDATE Usuarios 
                SET estado = 0
                WHERE Id = @id ";
            command.Parameters.AddWithValue("@id", usuario.Id);

            int filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }
        finally { _contexto.Close(); }
    }

}
