using Backend.Models;
using Backend.Persistence.Context;

namespace Backend.Persistence.Repositories;
public class ClienteRepository:IClienteRepository{
    private readonly ConexionSql _contexto;

    public ClienteRepository(ConexionSql contexto){
        _contexto = contexto;
    }

    public List<Cliente> ObtenerTodos(){
        var clientes = new List<Cliente>();
        try{
            _contexto.Open();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = "SELECT Id, Identificacion, Nombre, Telefono, Correo FROM Clientes";
            
            using var reader = command.ExecuteReader();
            while (reader.Read()){
                clientes.Add(new Cliente {
                    Id = reader.GetInt32(0),
                    Identificacion = reader.GetString(1),
                    Nombre = reader.GetString(2),
                    Telefono = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Correo = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }
        }
        finally { _contexto.Close(); }
        return clientes;
    }

    public int Crear(Cliente cliente){
        try{
            _contexto.Open();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = @"
                INSERT INTO Clientes (Identificacion, Nombre, Telefono, Correo) 
                VALUES (@ident, @nom, @tel, @corr);
                SELECT last_insert_rowid();";
            
            command.Parameters.AddWithValue("@ident", cliente.Identificacion);
            command.Parameters.AddWithValue("@nom", cliente.Nombre);
            command.Parameters.AddWithValue("@tel", (object)cliente.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@corr", (object)cliente.Correo ?? DBNull.Value);

            return Convert.ToInt32(command.ExecuteScalar());
        }
        finally { _contexto.Close(); }
    }
}
