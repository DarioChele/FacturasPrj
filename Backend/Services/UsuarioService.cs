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
        string hash = BCrypt.Net.BCrypt.HashPassword(UsuarioNuevo.PasswordHash);

        // 2. Mapeamos al modelo de base de datos
        var nuevoUsuario = new Usuario {
            Nombre = UsuarioNuevo.Nombre,
            Rol = UsuarioNuevo.Rol,
            PasswordHash = hash // <--- Aquí va el hash
        };
        // 3. Mandamos al repo a guardar
        return await _repo.Crear(nuevoUsuario);
    }
}