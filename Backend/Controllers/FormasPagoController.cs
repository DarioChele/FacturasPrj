using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

[Authorize] // <--- Solo usuarios con Token válido pueden entrar aquí
[ApiController]
[Route("api/[controller]")]
public class FormasPagoController : ControllerBase{
    private readonly IFormasPagoRepository _repository;
    private readonly ILogger<FormasPagoController> _logger;

    public FormasPagoController(IFormasPagoRepository repository, ILogger<FormasPagoController> logger){
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _repository.ObtenerTodos());

    
}