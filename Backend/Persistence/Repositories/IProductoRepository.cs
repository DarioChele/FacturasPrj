using Backend.Models;

namespace Backend.Persistence.Repositories;
public interface IProductoRepository{
    List<Producto> ObtenerTodos();
    int Crear(Producto producto);
    // Aquí añadir: Producto ? ObtenerPorId(int id);
}