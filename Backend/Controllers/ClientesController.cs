using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Persistence.Repositories;
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase{
    private readonly ClienteRepository _repository;

    public ClientesController(ClienteRepository repository){
        _repository = repository;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_repository.ObtenerTodos());

    [HttpPost]
    public IActionResult Post([FromBody] Cliente cliente){
        var id = _repository.Crear(cliente);
        return Ok(new { Message = "Creado", Id = id });
    }
}