using Backend.Persistence.Repositories;
using Backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Serilog;

namespace Backend.Services;

public class AuthService : IAuthService {
    private readonly IUsuarioRepository _userRepo;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUsuarioRepository userRepo, IConfiguration config, ILogger<AuthService> logger) {
        _userRepo = userRepo;
        _config = config;
        _logger = logger;
    }

    public async Task<string?> Login(string User, string Pwd) {
        var usuario = await _userRepo.ObtenerPorNombre(User);
        
        // Esto es para generar el hash de una contraseña y grabar en la BD//
        ////////////////////////////////////////////////////////////////////
        /// {
        ///   "user": "Dario",
        ///   "pwd": "admin123"
        /// }
        ///                                                     ////////////
            string passwordHash = GenerarHash(Pwd);             ////////////
            Console.WriteLine($"Tu hash es: {passwordHash}");   ////////////
        ////////////////////////////////////////////////////////////////////
        
        // Verificamos si existe y si el hash coincide
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(Pwd, usuario.PasswordHash)) {
            return null;
        }
        _logger.LogInformation("Usuario {User} ha iniciado sesion ", User);
        return GenerarToken(usuario);
    }

    private string GenerarToken(Usuario usuario) {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Los "Claims" son la información que viajará dentro del token (puedes leerlos en Angular)
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()) // 0 Admin, 1 Vendedor
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8), // El token dura una jornada laboral
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public static string GenerarHash(string password) {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}