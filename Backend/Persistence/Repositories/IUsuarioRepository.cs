using Backend.Models;

namespace Backend.Persistence.Repositories;
public interface IUsuarioRepository{
    List<Usuario> ObtenerTodos();
    int Crear(Usuario usuario);
    // Aquí añadir: Usuario ? ObtenerPorId(int id);
}