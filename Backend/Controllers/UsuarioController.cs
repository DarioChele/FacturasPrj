using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories;
using Backend.Persistence.Logging;
[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase{
    private readonly IUsuarioRepository _repository;
    private readonly IFileLogger _logger;

    public UsuarioController(IUsuarioRepository repository, IFileLogger logger){
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_repository.ObtenerTodos());

    [HttpPost]
    public IActionResult Post([FromBody] Usuario usuario){
        if (usuario == null) return BadRequest(); // Sin mensajes por seguridad

        try{
            var id = _repository.Crear(usuario);
            // 201 Created es lo ideal para POST exitosos
            return StatusCode(201, new { Id = id }); 
        }catch (Exception ex){
            _logger.LogError(ex, $"Fallo al crear usuario: {usuario?.Nombre}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
}