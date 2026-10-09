using CafeApi.DTOs;
using CafeApi.Interfaces;
using Npgsql;

namespace CafeApi.Repositories
{
    // ✅ Implementación de la gestión de pagos.
    public class PaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;

        public PaymentRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        // ✅ Crear pago real.
        public async Task<PaymentResponseDto?> CreatePaymentAsync(
            int orderId,
            int userId)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            // ✅ Obtener total del pedido.
            await using var orderCommand =
                new NpgsqlCommand(
                    @"SELECT total
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

            var totalResult =
                await orderCommand.ExecuteScalarAsync();

            if (totalResult == null)
            {
                return null;
            }

            var total =
                Convert.ToDecimal(totalResult);

            // ✅ Crear pago.
            await using var paymentCommand =
                new NpgsqlCommand(
                    @"INSERT INTO public.payments
                    (
                        order_id,
                        user_id,
                        monto,
                        estado,
                        fecha_creacion
                    )
                    VALUES
                    (
                        @orderId,
                        @userId,
                        @monto,
                        'Pendiente',
                        CURRENT_TIMESTAMP
                    )
                    RETURNING id;",
                    connection);

            paymentCommand.Parameters.AddWithValue(
                "orderId",
                orderId);

            paymentCommand.Parameters.AddWithValue(
                "userId",
                userId);

            paymentCommand.Parameters.AddWithValue(
                "monto",
                total);

            var paymentIdResult =
                await paymentCommand.ExecuteScalarAsync();

            var paymentId =
                Convert.ToInt32(paymentIdResult);

            return new PaymentResponseDto
            {
                PaymentId =
                    paymentId,

                OrderId =
                    orderId,

                UserId =
                    userId,

                Monto =
                    total,

                Estado =
                    "Pendiente",

                FechaCreacion =
                    DateTime.UtcNow
            };
        }

        // ✅ Obtener pago real.
        public async Task<PaymentResponseDto?> GetPaymentByOrderIdAsync(
            int orderId,
            int userId)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command =
                new NpgsqlCommand(
                    @"SELECT
                        id,
                        order_id,
                        user_id,
                        monto,
                        estado,
                        fecha_creacion
                      FROM public.payments
                      WHERE order_id = @orderId
                      AND user_id = @userId
                      ORDER BY id DESC
                      LIMIT 1;",
                    connection);

            command.Parameters.AddWithValue(
                "orderId",
                orderId);

            command.Parameters.AddWithValue(
                "userId",
                userId);

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new PaymentResponseDto
            {
                PaymentId =
                    reader.GetInt32(0),

                OrderId =
                    reader.GetInt32(1),

                UserId =
                    reader.GetInt32(2),

                Monto =
                    reader.GetDecimal(3),

                Estado =
                    reader.GetString(4),

                FechaCreacion =
                    reader.GetDateTime(5)
            };
        }
    }
}