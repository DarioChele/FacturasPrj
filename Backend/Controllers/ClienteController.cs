using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories;
using Backend.Persistence.Logging;
[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase{
    private readonly IClienteRepository _repository;
    private readonly IFileLogger _logger;

    public ClienteController(IClienteRepository repository, IFileLogger logger){
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_repository.ObtenerTodos());

    [HttpPost]
    public IActionResult Post([FromBody] Cliente cliente){
        if (cliente == null) return BadRequest();

        try{
            var id = _repository.Crear(cliente);
            // 201 Created es lo ideal para POST exitosos
            return StatusCode(201, new { Id = id }); 
        }catch (Exception ex){
            _logger.LogError(ex, $"Fallo al crear cliente: {cliente?.Nombre}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
}