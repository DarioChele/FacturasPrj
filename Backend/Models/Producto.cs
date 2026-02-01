namespace Backend.Models;
public class Producto{
    public int Id { get; set; }        
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Estado { get; set; }
}

public class ProductoDTO{
    public int Id { get; set; }        
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Estado { get; set; }
    public string EstadoDescripcion => Estado == 1 ? "Activo" : "Inactivo";
}

