using Backend.Models;

namespace Backend.Persistence.Repositories.Interfaces;
public interface IProveedorRepository{    
    Task<List<ProveedorDTO>> ObtenerTodos(string? estado = null, string? identificacion = null);
    Task<int> Crear(Proveedor proveedor);
    Task<bool> Modificar(Proveedor proveedor);
    Task<bool> Eliminar(Proveedor proveedor);
}