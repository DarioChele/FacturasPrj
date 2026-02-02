using Backend.Models;
public interface IFormasPagoRepository {
    Task<List<FormasPago>> ObtenerTodos();
}