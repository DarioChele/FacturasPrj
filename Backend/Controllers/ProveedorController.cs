using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

[Authorize] // <--- Solo usuarios con Token válido pueden entrar aquí
[ApiController]
[Route("api/[controller]")]
public class ProveedorController : ControllerBase{
    private readonly IProveedorRepository _repository;
    private readonly ILogger<ProveedorController> _logger;

    public ProveedorController(IProveedorRepository repository, ILogger<ProveedorController> logger){
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
            [FromQuery] string? estado, 
            [FromQuery] string? identificacion) 
            => Ok(await _repository.ObtenerTodos(estado, identificacion));


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Proveedor proveedor){
        if (proveedor == null) return BadRequest();

        try{
            var id = await _repository.Crear(proveedor);
            // 201 Created es lo ideal para POST exitosos
            return StatusCode(201, new { Id = id }); 
        }catch (Exception ex){
            _logger.LogError(ex, $"Fallo al crear proveedor: {proveedor?.Nombre}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
    [HttpPut("{id}")] 
    public async Task<IActionResult> Put(int id, [FromBody] Proveedor proveedor) {
        // Verificamos que el ID de la URL coincida con el del objeto
        if (proveedor == null || id != proveedor.Id) return BadRequest("ID inconsistente");

        try {
            bool actualizado = await _repository.Modificar(proveedor);
            if (!actualizado) return NotFound(); // Proveedor no encontrado            
            return StatusCode(201); 
        } catch (Exception ex) {
            _logger.LogError(ex, $"Fallo al actualizar proveedor ID: {id}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "0")] // Solo Admin puede eliminar productos
    public async Task<IActionResult> Delete(int id) {
        Proveedor proveedor = new Proveedor { Id = id };
        try {
            bool eliminado = await _repository.Eliminar(proveedor);
            if (!eliminado) return NotFound(); // Proveedor no encontrado            
            return StatusCode(201);
        } catch (Exception ex) {
            _logger.LogError(ex, $"Fallo al eliminar proveedor ID: {id}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
}