namespace Backend.Models;
public class Cliente{
    public int Id { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public int Estado { get; set; }
}
public class ClienteDTO{
    public int Id { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public int Estado { get; set; }
    public string EstadoDescripcion => Estado == 1 ? "Activo" : "Inactivo";
}
