using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories;
using Backend.Services;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger) {
        _authService = authService;
        _logger = logger;
    }
    /// <summary> 
    /// Inicia sesión de usuario con credenciales. 
    /// </summary> 
    /// <remarks>
    /// **Sample Request:**
    /// 
    ///     {
    ///       "user": "Tom",
    ///       "pwd": "tom321"
    ///     }
    ///     
    /// Retorna un JWT.
    /// </remarks>
    /// <param name="login">Objeto con usuario y contraseña.</param> 
    /// <returns>Token JWT si la autenticación es exitosa.</returns>
    /// <response code="200">Devuelve el token JWT</response>
    /// <response code="401">Credenciales inválidas</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO login) {
        var token = await _authService.Login(login.User, login.Pwd);
        
        if (token == null) {
            _logger.LogWarning("Error de autenticación para usuario {User}", login.User);
            return Unauthorized(new { Message = "Usuario o contraseña incorrectos" });                        
        }

        return Ok(new { Token = token });
    }
}
