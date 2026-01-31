namespace Backend.Models;
public class Factura {
    public int Id { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;    
    public decimal MontoTotal { get; set; }
    // Propiedad de navegación: Una factura tiene muchos detalles
    public List<DetalleFactura> Detalles { get; set; } = new();
    public List<PagoFactura> Pagos { get; set; } = new();
}
public class DetalleFactura {
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal PrecioTotal { get; set; }    
}
public class PagoFactura {
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public int FormaPagoId { get; set; }    
    public decimal ValorPagado { get; set; }
}