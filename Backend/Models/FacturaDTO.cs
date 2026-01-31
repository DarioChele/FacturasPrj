namespace Backend.Models;
public class FacturaDTO {
    public int Id { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;    
    public decimal MontoTotal { get; set; }
    // Propiedad de navegación: Una factura tiene muchos detalles
    public List<DetalleFacturaDTO> Detalles { get; set; } = new();
    public List<PagoFacturaDTO> Pagos { get; set; } = new();
}
public class DetalleFacturaDTO {
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal PrecioTotal { get; set; }    
}
public class PagoFacturaDTO {
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public int FormaPagoId { get; set; }    
    public string FormaPagoNombre { get; set; } = string.Empty;
    public decimal ValorPagado { get; set; }
}