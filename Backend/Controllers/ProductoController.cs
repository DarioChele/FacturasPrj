using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;

[Authorize] // <--- Solo usuarios con Token válido pueden entrar aquí
[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase{ 
    private readonly IProductoRepository _repository;
    private readonly ILogger<ProductoController>  _logger;

    public ProductoController(IProductoRepository repository, ILogger<ProductoController> logger) {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
            [FromQuery] string? estado,
            [FromQuery] string? id) 
            => Ok(await _repository.ObtenerTodos(estado, id));
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Producto producto){
        if (producto == null) return BadRequest(); 
        if (producto.Proveedores == null || !producto.Proveedores.Any()) return BadRequest(); 

        try{
            var id = await _repository.Crear(producto);
            // 201 Created es lo ideal para POST exitosos
            return StatusCode(201, new { Id = id }); 
        }catch (Exception ex){
            _logger.LogError(ex, $"Fallo al crear producto: {producto?.Nombre}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
    
    [HttpPut("{id}")] 
    public async Task<IActionResult> Put(int id, [FromBody] Producto producto) {
        // Verificamos que el ID de la URL coincida con el del objeto
        if (producto == null || id != producto.Id) return BadRequest("ID inconsistente");
        if (producto.Proveedores == null || !producto.Proveedores.Any()) return BadRequest(); 
        try {
            bool actualizado = await _repository.Modificar(producto);
            if (!actualizado) return NotFound(); // Producto no encontrado            
            return StatusCode(201); 
        } catch (Exception ex) {
            _logger.LogError(ex, $"Fallo al actualizar producto ID: {id}");
            return StatusCode(500, new { Message = "Err." });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "0")] // Solo Admin puede eliminar productos
    public async Task<IActionResult> Delete(int id) {
        Producto producto = new Producto { Id = id };
        try {
            bool eliminado = await _repository.Eliminar(producto);
            if (!eliminado) return NotFound(); // Producto no encontrado            
            return StatusCode(201);
        } catch (Exception ex) {
            _logger.LogError(ex, $"Fallo al eliminar producto ID: {id}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
}