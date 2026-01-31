using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories;
using Backend.Persistence.Logging;
[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase{ 
    private readonly IProductoRepository _repository;
    private readonly IFileLogger _logger;

    public ProductoController(IProductoRepository repository, IFileLogger logger) {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_repository.ObtenerTodos());
    
    [HttpPost]
    public IActionResult Post([FromBody] Producto producto){
        if (producto == null) return BadRequest(); // Sin mensajes innecesarios

        try{
            var id = _repository.Crear(producto);
            // 201 Created es lo ideal para POST exitosos
            return StatusCode(201, new { Id = id }); 
        }catch (Exception ex){
            _logger.LogError(ex, $"Fallo al crear producto: {producto?.Nombre}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
}