namespace CafeApi.DTOs
{
    // ✅ Solicitud para iniciar un pago.
    public class PaymentRequestDto
    {
        // ✅ Pedido que se desea pagar.
        public int OrderId { get; set; }
    }
}