namespace CafeApi.Models
{
    // ✅ Representa un producto dentro de un pedido.
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int CafeId { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}