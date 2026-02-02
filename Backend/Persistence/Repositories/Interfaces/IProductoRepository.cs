using Backend.Models;

namespace Backend.Persistence.Repositories;
public interface IProductoRepository{    
    Task<List<ProductoDTO>> ObtenerTodos(string? estado = null);
    Task<int> Crear(Producto producto);
    Task<bool> Modificar(Producto producto);
    Task<bool> Eliminar(Producto producto);
}