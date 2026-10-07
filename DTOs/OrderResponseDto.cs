namespace CafeApi.DTOs
{
    // ✅ Respuesta completa de un pedido.
    public class OrderResponseDto
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public string Estado { get; set; }
            = string.Empty;

        public decimal Total { get; set; }

        // ✅ Observaciones del pedido.
        public string Observaciones { get; set; }
            = string.Empty;

        public DateTime FechaCreacion { get; set; }

        public List<OrderItemResponseDto> Items
        { get; set; } = new();
    }
}