namespace Backend.Models;
public class Factura {
    public string NumeroFactura { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;    
    public decimal MontoTotal { get; set; }
    public int EstadoPago { get; set; } = 1; // 0: PendienteCobro, 1: Pagado, 2: Cancelado
    public int EstadoFactura { get; set; } = 1; // 1: Activo, 0: Inactivo
    public List<DetalleFactura> Detalles { get; set; } = new();
    public List<PagoFactura> Pagos { get; set; } = new();
}
public class DetalleFactura {
    public int FacturaId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal PrecioTotal { get; set; }    
}
public class PagoFactura {
    public int FacturaId { get; set; }
    public int FormaPagoId { get; set; }    
    public decimal ValorPagado { get; set; }
}