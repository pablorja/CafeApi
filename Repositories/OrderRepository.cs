using CafeApi.DTOs;
using CafeApi.Interfaces;
using Npgsql;

namespace CafeApi.Repositories
{
    // ✅ Implementación del repositorio de pedidos.
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        // ✅ Crear pedido desde el carrito.
        public async Task<OrderResponseDto?> CreateOrderAsync(
            int userId,
            string observaciones)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var transaction =
                await connection.BeginTransactionAsync();

            try
            {
                // ✅ Buscar carrito activo.
                await using var cartCommand =
                    new NpgsqlCommand(
                        @"SELECT id
                          FROM public.carts
                          WHERE user_id = @userId
                          AND estado = 'Activo'
                          LIMIT 1;",
                        connection,
                        transaction);

                cartCommand.Parameters.AddWithValue(
                    "userId",
                    userId);

                var cartIdResult =
                    await cartCommand.ExecuteScalarAsync();

                if (cartIdResult == null)
                {
                    return null;
                }

                var cartId =
                    Convert.ToInt32(cartIdResult);

                // ✅ Obtener items.
                await using var itemsCommand =
                    new NpgsqlCommand(
                        @"SELECT
                            ci.cafe_id,
                            c.nombre,
                            ci.precio_unitario,
                            ci.cantidad
                          FROM public.cart_items ci
                          INNER JOIN public.cafes c
                            ON c.id = ci.cafe_id
                          WHERE ci.cart_id = @cartId;",
                        connection,
                        transaction);

                itemsCommand.Parameters.AddWithValue(
                    "cartId",
                    cartId);

                await using var reader =
                    await itemsCommand.ExecuteReaderAsync();

                var items =
                    new List<OrderItemResponseDto>();

                decimal total = 0;

                while (await reader.ReadAsync())
                {
                    var cafeId =
                        reader.GetInt32(0);

                    var nombre =
                        reader.GetString(1);

                    var precio =
                        reader.GetDecimal(2);

                    var cantidad =
                        reader.GetInt32(3);

                    var subtotal =
                        precio * cantidad;

                    total += subtotal;

                    items.Add(
                        new OrderItemResponseDto
                        {
                            CafeId = cafeId,
                            CafeNombre = nombre,
                            PrecioUnitario = precio,
                            Cantidad = cantidad,
                            Subtotal = subtotal
                        });
                }

                await reader.CloseAsync();

                if (!items.Any())
                {
                    return null;
                }

                // ✅ Crear pedido.
                await using var orderCommand =
                    new NpgsqlCommand(
                        @"INSERT INTO public.orders
                        (
                            user_id,
                            total,
                            estado,
                            fecha_creacion
                        )
                        VALUES
                        (
                            @userId,
                            @total,
                            'PendientePago',
                            CURRENT_TIMESTAMP
                        )
                        RETURNING id;",
                        connection,
                        transaction);

                orderCommand.Parameters.AddWithValue(
                    "userId",
                    userId);

                orderCommand.Parameters.AddWithValue(
                    "total",
                    total);

                var orderIdResult =
                    await orderCommand.ExecuteScalarAsync();

                var orderId =
                    Convert.ToInt32(orderIdResult);

                // ✅ Crear detalle del pedido.
                foreach (var item in items)
                {
                    await using var detailCommand =
                        new NpgsqlCommand(
                            @"INSERT INTO public.order_items
                            (
                                order_id,
                                cafe_id,
                                cantidad,
                                precio_unitario,
                                subtotal
                            )
                            VALUES
                            (
                                @orderId,
                                @cafeId,
                                @cantidad,
                                @precioUnitario,
                                @subtotal
                            );",
                            connection,
                            transaction);

                    detailCommand.Parameters.AddWithValue(
                        "orderId",
                        orderId);

                    detailCommand.Parameters.AddWithValue(
                        "cafeId",
                        item.CafeId);

                    detailCommand.Parameters.AddWithValue(
                        "cantidad",
                        item.Cantidad);

                    detailCommand.Parameters.AddWithValue(
                        "precioUnitario",
                        item.PrecioUnitario);

                    detailCommand.Parameters.AddWithValue(
                        "subtotal",
                        item.Subtotal);

                    await detailCommand.ExecuteNonQueryAsync();
                }

                // ✅ Vaciar carrito.
                await using var clearCommand =
                    new NpgsqlCommand(
                        @"DELETE
                          FROM public.cart_items
                          WHERE cart_id = @cartId;",
                        connection,
                        transaction);

                clearCommand.Parameters.AddWithValue(
                    "cartId",
                    cartId);

                await clearCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return new OrderResponseDto
                {
                    OrderId = orderId,
                    UserId = userId,
                    Estado = "PendientePago",
                    Total = total,
                    FechaCreacion = DateTime.UtcNow,
                    Items = items
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ✅ Obtener pedidos de un usuario.
        public async Task<IEnumerable<OrderResponseDto>>
            GetOrdersByUserIdAsync(
                int userId)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command =
                new NpgsqlCommand(
                    @"SELECT
                        id,
                        total,
                        estado,
                        fecha_creacion
                      FROM public.orders
                      WHERE user_id = @userId
                      ORDER BY fecha_creacion DESC;",
                    connection);

            command.Parameters.AddWithValue(
                "userId",
                userId);

            await using var reader =
                await command.ExecuteReaderAsync();

            var orders =
                new List<OrderResponseDto>();

            while (await reader.ReadAsync())
            {
                orders.Add(
                    new OrderResponseDto
                    {
                        OrderId =
                            reader.GetInt32(0),

                        UserId =
                            userId,

                        Total =
                            reader.GetDecimal(1),

                        Estado =
                            reader.GetString(2),

                        FechaCreacion =
                            reader.GetDateTime(3)
                    });
            }

            return orders;
        }

        // ✅ Obtener pedido individual.
        public async Task<OrderResponseDto?> GetOrderByIdAsync(
            int orderId,
            int userId)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var orderCommand =
                new NpgsqlCommand(
                    @"SELECT
                        id,
                        total,
                        estado,
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

            var response =
                new OrderResponseDto
                {
                    OrderId =
                        reader.GetInt32(0),

                    UserId =
                        userId,

                    Total =
                        reader.GetDecimal(1),

                    Estado =
                        reader.GetString(2),

                    FechaCreacion =
                        reader.GetDateTime(3)
                };

            await reader.CloseAsync();

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
                response.Items.Add(
                    new OrderItemResponseDto
                    {
                        CafeId =
                            itemsReader.GetInt32(0),

                        CafeNombre =
                            itemsReader.GetString(1),

                        PrecioUnitario =
                            itemsReader.GetDecimal(2),

                        Cantidad =
                            itemsReader.GetInt32(3),

                        Subtotal =
                            itemsReader.GetDecimal(4)
                    });
            }

            return response;
        }
    }
}