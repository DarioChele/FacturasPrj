using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Persistence.Logging;
using Microsoft.AspNetCore.Authorization;

[Authorize] // <--- Solo usuarios con Token válido pueden entrar aquí
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
    public async Task<IActionResult> Get() => Ok(await _repository.ObtenerTodos());

    [HttpGet("{identificacion}")]
    public async Task<IActionResult> Get(string identificacion) {
        try {
            // Pasamos los filtros al repositorio
            var cliente = await _repository.ObtenerPorIdentificacion(identificacion);
            return Ok(cliente);
        } catch (Exception ex){
            _logger.LogError(ex, "Error al buscar cliente por identificacion");
            return StatusCode(500, new { Message = "Err." });
        }
    }


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Cliente cliente){
        if (cliente == null) return BadRequest();

        try{
            var id = await _repository.Crear(cliente);
            // 201 Created es lo ideal para POST exitosos
            return StatusCode(201, new { Id = id }); 
        }catch (Exception ex){
            _logger.LogError(ex, $"Fallo al crear cliente: {cliente?.Nombre}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
    [HttpPut("{id}")] 
    public async Task<IActionResult> Put(int id, [FromBody] Cliente cliente) {
        // Verificamos que el ID de la URL coincida con el del objeto
        if (cliente == null || id != cliente.Id) return BadRequest("ID inconsistente");

        try {
            bool actualizado = await _repository.Modificar(cliente);
            if (!actualizado) return NotFound(); // Cliente no encontrado            
            return StatusCode(201); 
        } catch (Exception ex) {
            _logger.LogError(ex, $"Fallo al actualizar cliente ID: {id}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "0")] // Solo Admin puede eliminar productos
    public async Task<IActionResult> Delete(int id) {
        Cliente cliente = new Cliente { Id = id };
        try {
            bool eliminado = await _repository.Eliminar(cliente);
            if (!eliminado) return NotFound(); // Cliente no encontrado            
            return StatusCode(201);
        } catch (Exception ex) {
            _logger.LogError(ex, $"Fallo al eliminar cliente ID: {id}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
}