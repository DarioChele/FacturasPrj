using Backend.Models;

namespace Backend.Persistence.Repositories;
public interface IUsuarioRepository{
    Task<List<UsuarioDTO>> ObtenerTodos();
    Task<Usuario?> ObtenerPorNombre(string nombre);
    Task<int> Crear(Usuario usuario);
    Task<bool> Modificar(Usuario usuario);
    Task<bool> Eliminar(Usuario usuario);
}