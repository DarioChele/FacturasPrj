using Backend.Models;

namespace Backend.Persistence.Repositories.Interfaces;
public interface IClienteRepository{    
    Task<List<ClienteDTO>> ObtenerTodos(string? estado = null, string? identificacion = null);
    Task<int> Crear(Cliente cliente);
    Task<bool> Modificar(Cliente cliente);
    Task<bool> Eliminar(Cliente cliente);
}