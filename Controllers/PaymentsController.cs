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
    public class PaymentsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly IPaymentRepository _paymentRepository;

        public PaymentsController(
            IUserRepository userRepository,
            IPaymentRepository paymentRepository)
        {
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
        }

        // ✅ Crear intención de pago.
        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromBody] PaymentRequestDto dto)
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

            var payment =
                await _paymentRepository
                    .CreatePaymentAsync(
                        dto.OrderId,
                        user.Id);

            if (payment == null)
            {
                return BadRequest(
                    "No fue posible crear el pago.");
            }

            return Ok(payment);
        }

        // ✅ Consultar pago por pedido.
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetPayment(
            int orderId)
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

            var payment =
                await _paymentRepository
                    .GetPaymentByOrderIdAsync(
                        orderId,
                        user.Id);

            if (payment == null)
            {
                return NotFound(
                    "Pago no encontrado.");
            }

            return Ok(payment);
        }
    }
}