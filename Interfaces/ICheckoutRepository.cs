using CafeApi.DTOs;

namespace CafeApi.Interfaces
{
    // ✅ Contrato para el proceso de checkout.
    public interface ICheckoutRepository
    {
        // ✅ Obtiene el resumen previo al pago.
        Task<CheckoutResponseDto?> GetCheckoutAsync(
            int orderId,
            int userId
        );
    }
}