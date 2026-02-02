using Backend.Models;
using Backend.Persistence.Context;
using Backend.Persistence.Repositories.Interfaces;

namespace Backend.Persistence.Repositories;
public class FormasPagoRepository:IFormasPagoRepository{
    private readonly ConexionSql _contexto;

    public FormasPagoRepository(ConexionSql contexto){
        _contexto = contexto;
    }

    public async Task<List<FormasPago>> ObtenerTodos(){
        var formasPago = new List<FormasPago>();
        try{
            await _contexto.OpenAsync();
            var command = _contexto.ObtenerConexion().CreateCommand();
            command.CommandText = "SELECT Id, TipoPago FROM FormasPago";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()){
                formasPago.Add(new FormasPago {                    
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TipoPago = reader.GetString(reader.GetOrdinal("TipoPago"))
                });
            }
        }
        finally { _contexto.Close(); }
        // Transformamos la lista de entidades a DTOs

        return formasPago.Select(c => new FormasPago {
            Id = c.Id,
            TipoPago = c.TipoPago
        }).ToList();        
    }
}