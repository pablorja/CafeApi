using CafeApi.DTOs;
using CafeApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly IOrderRepository _orderRepository;

        public OrdersController(
            IUserRepository userRepository,
            IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }

        // ✅ Crear pedido.
        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromBody] CreateOrderDto dto)
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

            var order =
                await _orderRepository
                    .CreateOrderAsync(
                        user.Id,
                        dto.Observaciones
                    );

            if (order == null)
            {
                return BadRequest(
                    "No fue posible generar el pedido.");
            }

            return Ok(order);
        }

        // ✅ Obtener mis pedidos.
        [HttpGet]
        public async Task<IActionResult> GetOrders()
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

            var orders =
                await _orderRepository
                    .GetOrdersByUserIdAsync(
                        user.Id
                    );

            return Ok(orders);
        }

        // ✅ Obtener pedido específico.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(
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

            var order =
                await _orderRepository
                    .GetOrderByIdAsync(
                        id,
                        user.Id
                    );

            if (order == null)
            {
                return NotFound(
                    "Pedido no encontrado.");
            }

            return Ok(order);
        }
        // ✅ Actualizar estado del pedido.
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] UpdateOrderStatusDto dto)
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

            var updated =
                await _orderRepository
                    .UpdateStatusAsync(
                        id,
                        user.Id,
                        dto.Estado);

            if (!updated)
            {
                return NotFound(
                    "Pedido no encontrado.");
            }

            return Ok(
                new
                {
                    message =
                        "Estado actualizado correctamente.",
                    orderId =
                        id,
                    estado =
                        dto.Estado
                });
        }
    }
}