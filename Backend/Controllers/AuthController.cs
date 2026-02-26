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
