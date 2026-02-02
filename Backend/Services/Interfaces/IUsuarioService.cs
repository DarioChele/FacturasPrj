using Backend.Models;

namespace Backend.Services;
public interface IUsuarioService {
    // Devuelve el ID del usuario registrado si es exitoso, 0 si falla
    Task<int> RegistrarUsuario(Usuario UsuarioNuevo);
    Task<bool> ActualizarUsuario(Usuario UsuarioNuevo);
}