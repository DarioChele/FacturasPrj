using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories;
using Backend.Persistence.Logging;
using Backend.Services;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO login) {
        var token = await _authService.Login(login.User, login.Pwd);
        
        if (token == null) 
            return Unauthorized(new { Message = "Usuario o contraseña incorrectos" });

        return Ok(new { Token = token });
    }
}
