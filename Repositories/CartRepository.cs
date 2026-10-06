using CafeApi.DTOs;
using CafeApi.Interfaces;
using Npgsql;

namespace CafeApi.Repositories
{
    // ✅ Implementación del repositorio del carrito.
    public class CartRepository : ICartRepository
    {
        // ✅ Cadena de conexión PostgreSQL.
        private readonly string _connectionString;

        // ✅ Constructor.
        public CartRepository(
        string connectionString)
        {
            _connectionString = connectionString;
        }

        // ✅ Agrega un producto al carrito.
        public async Task AddItemAsync(
        int userId,
        int cafeId,
        int cantidad)
        {
            await using var connection =
            new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            // ✅ Buscar carrito activo.
            await using var cartCommand =
            new NpgsqlCommand(
            @"SELECT id
FROM public.carts
WHERE user_id = @userId
AND estado = 'Activo'
LIMIT 1;",
            connection);

            cartCommand.Parameters.AddWithValue(
            "userId",
            userId);

            var cartIdResult =
            await cartCommand.ExecuteScalarAsync();

            if (cartIdResult == null)
            {
                throw new InvalidOperationException(
                "No existe un carrito activo.");
            }

            var cartId =
            Convert.ToInt32(cartIdResult);

            // ✅ Obtener precio actual del café.
            await using var cafeCommand =
            new NpgsqlCommand(
            @"SELECT precio
FROM public.cafes
WHERE id = @cafeId;",
            connection);

            cafeCommand.Parameters.AddWithValue(
            "cafeId",
            cafeId);

            var precioResult =
            await cafeCommand.ExecuteScalarAsync();

            if (precioResult == null)
            {
                throw new InvalidOperationException(
                "El café no existe.");
            }

            var precio =
            Convert.ToDecimal(precioResult);

            // ✅ Insertar producto.
            await using var insertCommand =
            new NpgsqlCommand(
            @"INSERT INTO public.cart_items
(
cart_id,
cafe_id,
cantidad,
precio_unitario
)
VALUES
(
@cartId,
@cafeId,
@cantidad,
@precio
);",
            connection);

            insertCommand.Parameters.AddWithValue(
            "cartId",
            cartId);

            insertCommand.Parameters.AddWithValue(
            "cafeId",
            cafeId);

            insertCommand.Parameters.AddWithValue(
            "cantidad",
            cantidad);

            insertCommand.Parameters.AddWithValue(
            "precio",
            precio);

            await insertCommand.ExecuteNonQueryAsync();
        }

        // ✅ Obtiene el carrito completo del usuario.
        public async Task<CartResponseDto?> GetCartByUserIdAsync(
        int userId)
        {
            await using var connection =
            new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            // ✅ Buscar carrito activo.
            await using var cartCommand =
            new NpgsqlCommand(
            @"SELECT
id,
user_id
FROM public.carts
WHERE user_id = @userId
AND estado = 'Activo'
LIMIT 1;",
            connection);

            cartCommand.Parameters.AddWithValue(
            "userId",
            userId);

            await using var cartReader =
            await cartCommand.ExecuteReaderAsync();

            if (!await cartReader.ReadAsync())
            {
                return null;
            }

            var cartId =
            cartReader.GetInt32(0);

            var cartUserId =
            cartReader.GetInt32(1);

            await cartReader.CloseAsync();

            // ✅ Obtener items reales.
            await using var itemsCommand =
            new NpgsqlCommand(
            @"SELECT
ci.cafe_id,
c.nombre,
c.imagen_url,
ci.precio_unitario,
ci.cantidad
FROM public.cart_items ci
INNER JOIN public.cafes c
ON c.id = ci.cafe_id
WHERE ci.cart_id = @cartId;",
            connection);

            itemsCommand.Parameters.AddWithValue(
            "cartId",
            cartId);

            await using var itemsReader =
            await itemsCommand.ExecuteReaderAsync();

            var items =
            new List<CartItemResponseDto>();

            decimal total = 0;

            int cantidadItems = 0;

            while (await itemsReader.ReadAsync())
            {
                var precio =
                itemsReader.GetDecimal(3);

                var cantidad =
                itemsReader.GetInt32(4);

                var subtotal =
                precio * cantidad;

                total += subtotal;

                cantidadItems += cantidad;

                items.Add(
                new CartItemResponseDto
                {
                    CafeId =
                itemsReader.GetInt32(0),

                    CafeNombre =
                itemsReader.GetString(1),

                    ImagenUrl =
                itemsReader.IsDBNull(2)
                ? string.Empty
                : itemsReader.GetString(2),

                    Precio =
                precio,

                    Cantidad =
                cantidad,

                    Subtotal =
                subtotal
                });
            }

            return new CartResponseDto
            {
                CartId = cartId,
                UserId = cartUserId,
                Items = items,
                CantidadItems = cantidadItems,
                Total = total
            };
        }

        // ✅ Actualiza cantidad.
        public async Task<bool> UpdateQuantityAsync(
        int cartItemId,
        int cantidad)
        {
            await using var connection =
            new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command =
            new NpgsqlCommand(
            @"UPDATE public.cart_items
SET cantidad = @cantidad
WHERE id = @cartItemId;",
            connection);

            command.Parameters.AddWithValue(
            "cantidad",
            cantidad);

            command.Parameters.AddWithValue(
            "cartItemId",
            cartItemId);

            return
            await command.ExecuteNonQueryAsync() > 0;
        }

        // ✅ Elimina un producto.
        public async Task<bool> RemoveItemAsync(
        int cartItemId)
        {
            await using var connection =
            new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command =
            new NpgsqlCommand(
            @"DELETE FROM public.cart_items
WHERE id = @cartItemId;",
            connection);

            command.Parameters.AddWithValue(
            "cartItemId",
            cartItemId);

            return
            await command.ExecuteNonQueryAsync() > 0;
        }

        // ✅ Vacía completamente el carrito.
        public async Task<bool> ClearCartAsync(
        int userId)
        {
            await using var connection =
            new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var cartCommand =
            new NpgsqlCommand(
            @"SELECT id
FROM public.carts
WHERE user_id = @userId
AND estado = 'Activo'
LIMIT 1;",
            connection);

            cartCommand.Parameters.AddWithValue(
            "userId",
            userId);

            var cartIdResult =
            await cartCommand.ExecuteScalarAsync();

            if (cartIdResult == null)
            {
                return false;
            }

            var cartId =
            Convert.ToInt32(cartIdResult);

            await using var command =
            new NpgsqlCommand(
            @"DELETE
FROM public.cart_items
WHERE cart_id = @cartId;",
            connection);

            command.Parameters.AddWithValue(
            "cartId",
            cartId);

            await command.ExecuteNonQueryAsync();

            return true;
        }

        // ✅ Verifica si el item pertenece al usuario.
        public async Task<bool> ItemBelongsToUserAsync(
        int cartItemId,
        int userId)
        {
            await using var connection =
            new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            await using var command =
            new NpgsqlCommand(
            @"SELECT 1
FROM public.cart_items ci
INNER JOIN public.carts c
ON c.id = ci.cart_id
WHERE ci.id = @cartItemId
AND c.user_id = @userId;",
            connection);

            command.Parameters.AddWithValue(
            "cartItemId",
            cartItemId);

            command.Parameters.AddWithValue(
            "userId",
            userId);

            var result =
            await command.ExecuteScalarAsync();

            return result != null;
        }
    }
}