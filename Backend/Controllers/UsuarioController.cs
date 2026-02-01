using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories;
using Backend.Persistence.Logging;
using Microsoft.AspNetCore.Authorization;
using Backend.Services;
using Backend.Persistence.Repositories.Interfaces;

[Authorize] // <--- Solo usuarios con Token válido pueden entrar aquí
[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase{
    private readonly IUsuarioRepository _repository;
    private readonly IUsuarioService _usuarioService;   
    private readonly IFileLogger _logger;

    public UsuarioController(IUsuarioRepository repository, IUsuarioService usuarioService, IFileLogger logger){
        _repository = repository;
        _usuarioService = usuarioService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _repository.ObtenerTodos());

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Usuario usuario){
        if (usuario == null) return BadRequest(); // Sin mensajes por seguridad

        try{
            var id = await _usuarioService.RegistrarUsuario(usuario);
            // 201 Created es lo ideal para POST exitosos
            return StatusCode(201, new { Id = id }); 
        }catch (Exception ex){
            _logger.LogError(ex, $"Fallo al crear usuario: {usuario?.Nombre}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
    [HttpPut("{id}")] 
    public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario) {
        // Verificamos que el ID de la URL coincida con el del objeto
        if (usuario == null || id != usuario.Id) return BadRequest("ID inconsistente");

        try {
            bool actualizado = await _repository.Modificar(usuario);
            if (!actualizado) return NotFound(); // Usuario no encontrado            
            return StatusCode(201); 
        } catch (Exception ex) {
            _logger.LogError(ex, $"Fallo al actualizar usuario ID: {id}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "0")] // Solo Admin puede eliminar usuarios
    public async Task<IActionResult> Delete(int id) {
        Usuario usuario = new Usuario { Id = id };
        try {
            bool eliminado = await _repository.Eliminar(usuario);
            if (!eliminado) return NotFound(); // Usuario no encontrado            
            return StatusCode(201);
        } catch (Exception ex) {
            _logger.LogError(ex, $"Fallo al eliminar usuario ID: {id}");
            return StatusCode(500, new { Message = "Err." });
        }
    }
}