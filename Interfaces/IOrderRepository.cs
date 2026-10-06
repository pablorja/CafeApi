using CafeApi.DTOs;

namespace CafeApi.Interfaces
{
    // ✅ Contrato para la gestión de pedidos.
    public interface IOrderRepository
    {
        // ✅ Crear pedido desde el carrito.
        Task<OrderResponseDto?> CreateOrderAsync(
            int userId,
            string observaciones
        );

        // ✅ Obtener todos los pedidos del usuario.
        Task<IEnumerable<OrderResponseDto>>
            GetOrdersByUserIdAsync(
                int userId
            );

        // ✅ Obtener pedido concreto.
        Task<OrderResponseDto?> GetOrderByIdAsync(
            int orderId,
            int userId
        );
    }
}