namespace CafeApi.DTOs
{
    // ✅ Respuesta de un pago.
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public int UserId { get; set; }

        public decimal Monto { get; set; }

        public string Estado { get; set; }
            = string.Empty;

        public DateTime FechaCreacion { get; set; }
    }
}