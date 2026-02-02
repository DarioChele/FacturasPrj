using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Persistence.Logging;
using Microsoft.AspNetCore.Authorization;

[Authorize] // <--- Solo usuarios con Token válido pueden entrar aquí
[ApiController]
[Route("api/[controller]")]
public class FormasPagoController : ControllerBase{
    private readonly IFormasPagoRepository _repository;
    private readonly IFileLogger _logger;

    public FormasPagoController(IFormasPagoRepository repository, IFileLogger logger){
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _repository.ObtenerTodos());

    
}