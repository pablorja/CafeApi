namespace CafeApi.DTOs
{
    // ✅ Producto incluido en un pedido.
    public class OrderItemResponseDto
    {
        public int CafeId { get; set; }

        public string CafeNombre { get; set; }
            = string.Empty;

        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; }

        public decimal Subtotal { get; set; }
    }
}