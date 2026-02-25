namespace Backend.Models;
public class Producto{
    public int Id { get; set; }        
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Estado { get; set; }
    public List<DetalleProveedorDTO> Proveedores { get; set; } = new();
}

public class ProductoDTO{
    public int Id { get; set; }        
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Estado { get; set; }
    public string EstadoDescripcion => Estado == 1 ? "Activo" : "Inactivo";
    public int StockTotal { get; set; }
    public List<DetalleProveedorDTO> Proveedores { get; set; } = new();
}

public class DetalleProveedorDTO {
    public int ProveedorId { get; set; }     
    public string NombreProveedor { get; set; } = string.Empty;
    public decimal Precio { get; set; } 
    public int Stock { get; set; } 
    public string NumeroLote { get; set; } = string.Empty;
    
}