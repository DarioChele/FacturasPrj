using Microsoft.Data.Sqlite;
using Backend.Models;
using Backend.Persistence.Context;

namespace Backend.Persistence.Repositories{
    public class UsuarioRepository:IUsuarioRepository{
        private readonly ConexionSql _contexto;

        public UsuarioRepository(ConexionSql contexto){
        _contexto = contexto;
    }

        public List<Usuario> ObtenerTodos(){
            var usuarios = new List<Usuario>();
            try{
                _contexto.Open();
                var command = _contexto.ObtenerConexion().CreateCommand();
                command.CommandText = "SELECT Id, Nombre, Rol FROM Usuarios";
                
                using var reader = command.ExecuteReader();
                while (reader.Read()){
                    usuarios.Add(new Usuario {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Rol = reader.GetString(2)
                    });
                }
            }
            finally { _contexto.Close(); }
            return usuarios;
        }

        public int Crear(Usuario usuario){
            try{
                _contexto.Open();
                var command = _contexto.ObtenerConexion().CreateCommand();
                command.CommandText = @"
                    INSERT INTO Usuarios (Nombre, Rol) 
                    VALUES (@nom, @rol);
                    SELECT last_insert_rowid();";
                
                command.Parameters.AddWithValue("@nom", usuario.Nombre);
                command.Parameters.AddWithValue("@rol", usuario.Rol);
                return Convert.ToInt32(command.ExecuteScalar());
            }
            finally { _contexto.Close(); }
        }
    }
}