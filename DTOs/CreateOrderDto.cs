namespace CafeApi.DTOs
{
    // ✅ DTO utilizado para crear un pedido.
    public class CreateOrderDto
    {
        // ✅ Observaciones opcionales.
        public string Observaciones { get; set; }
            = string.Empty;
    }
}