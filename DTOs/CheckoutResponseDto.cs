namespace CafeApi.DTOs
{
    // ✅ Resumen previo al pago.
    public class CheckoutResponseDto
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public string Estado { get; set; }
            = string.Empty;

        public string Observaciones { get; set; }
            = string.Empty;

        public int CantidadItems { get; set; }

        public decimal Total { get; set; }

        public DateTime FechaCreacion { get; set; }

        // ✅ Determina si el pedido puede pagarse.
        public bool PuedePagar { get; set; }

        public List<OrderItemResponseDto> Items
        { get; set; } = new();
    }
}