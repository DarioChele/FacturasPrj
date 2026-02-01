
namespace Backend.Services;
public interface IAuthService {
    // Devuelve el Token JWT si es exitoso, null si falla
    Task<string?> Login(string User, string Pwd);
}