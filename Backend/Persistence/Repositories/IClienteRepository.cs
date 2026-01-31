using Backend.Models;

namespace Backend.Persistence.Repositories;
public interface IClienteRepository{
    List<Cliente> ObtenerTodos();
    int Crear(Cliente cliente);
    // Aquí añadir: Cliente? ObtenerPorId(int id);
}