using CafeApi.DTOs;
using CafeApi.Interfaces;
using Npgsql;

namespace CafeApi.Repositories
{
    // ✅ Implementación del proceso de checkout.
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly string _connectionString;

        public CheckoutRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        // ✅ Obtiene el resumen previo al pago.
        public async Task<CheckoutResponseDto?> GetCheckoutAsync(
            int orderId,
            int userId)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            // ✅ Buscar pedido.
            await using var orderCommand =
                new NpgsqlCommand(
                    @"SELECT
                        id,
                        user_id,
                        total,
                        estado,
                        observaciones,
                        fecha_creacion
                      FROM public.orders
                      WHERE id = @orderId
                      AND user_id = @userId;",
                    connection);

            orderCommand.Parameters.AddWithValue(
                "orderId",
                orderId);

            orderCommand.Parameters.AddWithValue(
                "userId",
                userId);

            await using var reader =
                await orderCommand.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            var checkout =
                new CheckoutResponseDto
                {
                    OrderId =
                        reader.GetInt32(0),

                    UserId =
                        reader.GetInt32(1),

                    Total =
                        reader.GetDecimal(2),

                    Estado =
                        reader.GetString(3),

                    Observaciones =
                        reader.IsDBNull(4)
                        ? string.Empty
                        : reader.GetString(4),

                    FechaCreacion =
                        reader.GetDateTime(5),

                    CantidadItems = 0
                };

            await reader.CloseAsync();

            // ✅ Obtener productos del pedido.
            await using var itemsCommand =
                new NpgsqlCommand(
                    @"SELECT
                        oi.cafe_id,
                        c.nombre,
                        oi.precio_unitario,
                        oi.cantidad,
                        oi.subtotal
                      FROM public.order_items oi
                      INNER JOIN public.cafes c
                        ON c.id = oi.cafe_id
                      WHERE oi.order_id = @orderId;",
                    connection);

            itemsCommand.Parameters.AddWithValue(
                "orderId",
                orderId);

            await using var itemsReader =
                await itemsCommand.ExecuteReaderAsync();

            while (await itemsReader.ReadAsync())
            {
                var cantidad =
                    itemsReader.GetInt32(3);

                checkout.CantidadItems +=
                    cantidad;

                checkout.Items.Add(
                    new OrderItemResponseDto
                    {
                        CafeId =
                            itemsReader.GetInt32(0),

                        CafeNombre =
                            itemsReader.GetString(1),

                        PrecioUnitario =
                            itemsReader.GetDecimal(2),

                        Cantidad =
                            cantidad,

                        Subtotal =
                            itemsReader.GetDecimal(4)
                    });
            }

            // ✅ Determinar si puede pagarse.
            checkout.PuedePagar =
                checkout.Estado == "PendientePago"
                && checkout.Total > 0
                && checkout.Items.Any();

            return checkout;
        }
    }
}