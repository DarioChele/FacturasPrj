namespace Backend.Models;
public class Usuario{
    public int Id { get; set; }        
    public string Nombre { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int Rol { get; set; }
    public int Estado { get; set; }
}

public class UsuarioDTO{
    public int Id { get; set; }        
    public string Nombre { get; set; } = string.Empty;
    public int Rol { get; set; }
    public string RolDescripcion => Rol == 0 ? "Administrador" : "Vendedor";
    public int Estado { get; set; }
    public string EstadoDescripcion => Estado == 1 ? "Activo" : "Inactivo";
}