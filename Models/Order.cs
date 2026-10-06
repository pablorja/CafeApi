namespace CafeApi.Models
{
    // ✅ Representa un pedido realizado por un usuario.
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal Total { get; set; }

        public string Estado { get; set; }
            = "PendientePago";

        public DateTime FechaCreacion { get; set; }

        // ✅ Observaciones del cliente.
        public string Observaciones { get; set; }
            = string.Empty;
    }
}