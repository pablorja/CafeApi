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
                // ✅ Buscar carrito.
                await using var cartCommand =
                    new NpgsqlCommand(
                        @"SELECT id
                          FROM public.carts
                          WHERE user_id = @userId
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

                // ✅ Obtener productos.
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
                            CafeId =
                                reader.GetInt32(0),

                            CafeNombre =
                                reader.GetString(1),

                            PrecioUnitario =
                                precio,

                            Cantidad =
                                cantidad,

                            Subtotal =
                                subtotal
                        });
                }

                await reader.CloseAsync();

                // ✅ Carrito vacío.
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
                            observaciones,
                            fecha_creacion
                        )
                        VALUES
                        (
                            @userId,
                            @total,
                            'PendientePago',
                            @observaciones,
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

                orderCommand.Parameters.AddWithValue(
                    "observaciones",
                    observaciones);

                var orderIdResult =
                    await orderCommand.ExecuteScalarAsync();

                var orderId =
                    Convert.ToInt32(orderIdResult);

                // ✅ Crear detalle.
                foreach (var item in items)
                {
                    await using var itemCommand =
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

                    itemCommand.Parameters.AddWithValue(
                        "orderId",
                        orderId);

                    itemCommand.Parameters.AddWithValue(
                        "cafeId",
                        item.CafeId);

                    itemCommand.Parameters.AddWithValue(
                        "cantidad",
                        item.Cantidad);

                    itemCommand.Parameters.AddWithValue(
                        "precioUnitario",
                        item.PrecioUnitario);

                    itemCommand.Parameters.AddWithValue(
                        "subtotal",
                        item.Subtotal);

                    await itemCommand.ExecuteNonQueryAsync();
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
                    OrderId =
                        orderId,

                    UserId =
                        userId,

                    Estado =
                        "PendientePago",

                    Total =
                        total,

                    Observaciones =
                        observaciones,

                    FechaCreacion =
                        DateTime.UtcNow,

                    Items =
                        items
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ✅ Obtener historial de pedidos.
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
                        observaciones,
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

                        Observaciones =
                            reader.IsDBNull(3)
                            ? string.Empty
                            : reader.GetString(3),

                        FechaCreacion =
                            reader.GetDateTime(4)
                    });
            }

            return orders;
        }

        // ✅ Obtener pedido específico.
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

                    Observaciones =
                        reader.IsDBNull(3)
                        ? string.Empty
                        : reader.GetString(3),

                    FechaCreacion =
                        reader.GetDateTime(4)
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