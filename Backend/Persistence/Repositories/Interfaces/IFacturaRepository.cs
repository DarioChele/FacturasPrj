using Backend.Models;

public interface IFacturaRepository {
    
    Task<List<FacturaDTO>> ObtenerTodos(string? numero = null, DateTime? fecha = null, decimal? monto = null);
    Task<int> Crear(Factura factura) ;
    Task<FacturaDTO?> ObtenerPorId(int id);
}