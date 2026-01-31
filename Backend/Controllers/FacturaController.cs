using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories;
using Backend.Persistence.Logging;
[ApiController]
[Route("api/[controller]")]
public class FacturaController : ControllerBase{
    private readonly IFacturaRepository _repository;
    private readonly IFileLogger _logger;

    public FacturaController(IFacturaRepository repository, IFileLogger logger){
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_repository.ObtenerTodos());

    [HttpPost]
    public IActionResult Post([FromBody] Factura factura){
        if (factura == null) return BadRequest();
        
        if (factura.Detalles == null || !factura.Detalles.Any()){
            return BadRequest(); //"La factura debe tener al menos un detalle."
        }
        try{
            var id = _repository.Crear(factura);
            // 201 Created es lo ideal para POST exitosos
            return StatusCode(201, new { Id = id }); 
        }catch (Exception ex){
            _logger.LogError(ex, $"Fallo al crear factura: {factura?.NumeroFactura}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
}