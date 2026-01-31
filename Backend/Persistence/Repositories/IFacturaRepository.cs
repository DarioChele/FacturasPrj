using Backend.Models;

public interface IFacturaRepository {
    
     List<FacturaDTO> ObtenerTodos();
    int Crear(Factura factura);
    Factura? ObtenerPorId(int id);
}