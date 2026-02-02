namespace Backend.Services;
using Backend.Models;
using Backend.Persistence.Repositories;
using Backend.Persistence.Repositories.Interfaces;
public class UsuarioService : IUsuarioService {
    private readonly IUsuarioRepository _repo;

    public UsuarioService(IUsuarioRepository repo) {
        _repo = repo;
    }

    public async Task<int> RegistrarUsuario(Usuario UsuarioNuevo) {
        // 1. Encriptamos la contraseña aquí, fuera del repo
        string hash = AuthService.GenerarHash(UsuarioNuevo.PasswordHash);
        // 2. Mapeamos al modelo de base de datos
        var nuevoUsuario = MapearUsuario(UsuarioNuevo, hash);
        // 3. Mandamos al repo a guardar
        return await _repo.Crear(nuevoUsuario);
    }
    public async Task<bool> ActualizarUsuario(Usuario UsuarioNuevo) {
        // 1. Encriptamos la contraseña aquí, fuera del repo
        string hash = AuthService.GenerarHash(UsuarioNuevo.PasswordHash);
        // 2. Mapeamos al modelo de base de datos
        var nuevoUsuario = MapearUsuario(UsuarioNuevo, hash);
        // 3. Mandamos al repo a guardar
        return await _repo.Modificar(nuevoUsuario);
    }
    private Usuario MapearUsuario(Usuario usuario, string hash) {
        return new Usuario {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol,
            PasswordHash = hash,
            Estado = usuario.Estado
        };
    }
}