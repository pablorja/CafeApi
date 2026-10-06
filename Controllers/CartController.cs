using CafeApi.DTOs;
using CafeApi.Interfaces;
using CafeApi.Models;
using CafeApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly ICartRepository _cartRepository;

        private readonly ICafeRepository _cafeRepository;

        public CartController(
        IUserRepository userRepository,
        ICartRepository cartRepository,
        ICafeRepository cafeRepository)
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _cafeRepository = cafeRepository;
        }

        // ✅ Obtener carrito actual.
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var email =
            User.FindFirstValue(
            ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(
                "No fue posible obtener el correo del token.");
            }

            var user =
            await _userRepository
            .GetByEmailAsync(email);

            if (user == null)
            {
                return NotFound(
                "Usuario no encontrado.");
            }

            var cart =
            await _cartRepository
            .GetCartByUserIdAsync(
            user.Id);

            if (cart == null)
            {
                return NotFound(
                "El usuario no tiene carrito activo.");
            }

            return Ok(cart);
        }

        // ✅ Agregar producto.
        [HttpPost("items")]
        public async Task<IActionResult> AddItem(
        [FromBody] AddCartItemDto dto)
        {
            var email =
            User.FindFirstValue(
            ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(
                "No fue posible obtener el correo del token.");
            }

            var user =
            await _userRepository
            .GetByEmailAsync(email);

            if (user == null)
            {
                return NotFound(
                "Usuario no encontrado.");
            }

            var cafe =
            _cafeRepository.GetById(
            dto.CafeId);

            if (cafe == null)
            {
                return NotFound(
                "El café no existe.");
            }

            if (dto.Cantidad <= 0)
            {
                return BadRequest(
                "La cantidad debe ser mayor que cero.");
            }

            if (dto.Cantidad > 100)
            {
                return BadRequest(
                "La cantidad máxima permitida es 100.");
            }

            if (cafe.Stock <= 0)
            {
                return BadRequest(
                "El café no tiene stock disponible.");
            }

            if (dto.Cantidad > cafe.Stock)
            {
                return BadRequest(
                "La cantidad solicitada supera el stock disponible.");
            }

            await _cartRepository.AddItemAsync(
            user.Id,
            dto.CafeId,
            dto.Cantidad);

            return Ok(new
            {
                Message =
            "Producto agregado al carrito correctamente."
            });
        }

        // ✅ Actualizar cantidad.
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateQuantity(
        int id,
        [FromBody] UpdateCartItemDto dto)
        {
            if (dto.Cantidad <= 0)
            {
                return BadRequest(
                "La cantidad debe ser mayor que cero.");
            }

            if (dto.Cantidad > 100)
            {
                return BadRequest(
                "La cantidad máxima permitida es 100.");
            }

            var email =
            User.FindFirstValue(
            ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(
                "No fue posible obtener el correo del token.");
            }

            var user =
            await _userRepository
            .GetByEmailAsync(email);

            if (user == null)
            {
                return NotFound(
                "Usuario no encontrado.");
            }

            var belongsToUser =
            await _cartRepository
            .ItemBelongsToUserAsync(
            id,
            user.Id);

            if (!belongsToUser)
            {
                return Forbid();
            }

            var updated =
            await _cartRepository
            .UpdateQuantityAsync(
            id,
            dto.Cantidad);

            if (!updated)
            {
                return NotFound(
                "Producto no encontrado.");
            }

            return Ok(new
            {
                Message =
            "Cantidad actualizada correctamente."
            });
        }

        // ✅ Eliminar producto.
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemoveItem(
        int id)
        {
            var email =
            User.FindFirstValue(
            ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(
                "No fue posible obtener el correo del token.");
            }

            var user =
            await _userRepository
            .GetByEmailAsync(email);

            if (user == null)
            {
                return NotFound(
                "Usuario no encontrado.");
            }

            var belongsToUser =
            await _cartRepository
            .ItemBelongsToUserAsync(
            id,
            user.Id);

            if (!belongsToUser)
            {
                return Forbid();
            }

            var removed =
            await _cartRepository
            .RemoveItemAsync(id);

            if (!removed)
            {
                return NotFound(
                "Producto no encontrado.");
            }

            return Ok(new
            {
                Message =
            "Producto eliminado correctamente."
            });
        }

        // ✅ Vaciar carrito.
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var email =
            User.FindFirstValue(
            ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(
                "No fue posible obtener el correo del token.");
            }

            var user =
            await _userRepository
            .GetByEmailAsync(email);

            if (user == null)
            {
                return NotFound(
                "Usuario no encontrado.");
            }

            var cleared =
            await _cartRepository
            .ClearCartAsync(
            user.Id);

            if (!cleared)
            {
                return NotFound(
                "No existe un carrito activo.");
            }

            return Ok(new
            {
                Message =
            "Carrito vaciado correctamente."
            });
        }
    }
}