using CafeApi.DTOs;

namespace CafeApi.Interfaces
{
    // ✅ Contrato para la gestión de pagos.
    public interface IPaymentRepository
    {
        // ✅ Crear una intención de pago.
        Task<PaymentResponseDto?> CreatePaymentAsync(
            int orderId,
            int userId
        );

        // ✅ Consultar un pago.
        Task<PaymentResponseDto?> GetPaymentByOrderIdAsync(
            int orderId,
            int userId
        );
    }
}