using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;

[Authorize] // <--- Solo usuarios con Token válido pueden entrar aquí
[ApiController]
[Route("api/[controller]")]
public class FacturaController : ControllerBase{
    private readonly IFacturaRepository _repository;
    private readonly ILogger<FacturaController> _logger;

    public FacturaController(IFacturaRepository repository, ILogger<FacturaController> logger){
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get( 
                                [FromQuery] string? numero, 
                                [FromQuery] DateTime? fecha, 
                                [FromQuery] decimal? monto){
        try {
            // Pasamos los filtros al repositorio
            var facturas = await _repository.ObtenerTodos(numero, fecha, monto);
            return Ok(facturas);
        } catch (Exception ex){
            _logger.LogError(ex, "Error al buscar facturas");
            return StatusCode(500, new { Message = "Err." });
        }
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id){
        try {
            // Pasamos los filtros al repositorio
            var factura = await _repository.ObtenerPorId(id);
            return Ok(factura);
        } catch (Exception ex){
            _logger.LogError(ex, "Error al buscar factura por ID");
            return StatusCode(500, new { Message = "Err." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Factura factura){
        if (factura == null) return BadRequest();
        
        if (factura.Detalles == null || !factura.Detalles.Any()){
            return BadRequest(); //"La factura debe tener al menos un detalle."
        }
        try{
            var id = await _repository.Crear(factura);
            // 201 Created es lo ideal para POST exitosos
            return StatusCode(201, new { Id = id }); 
        }catch (Exception ex){
            _logger.LogError(ex, $"Fallo al crear factura: {factura?.NumeroFactura}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
}