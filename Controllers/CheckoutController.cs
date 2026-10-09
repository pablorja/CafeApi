using CafeApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly ICheckoutRepository _checkoutRepository;

        public CheckoutController(
        IUserRepository userRepository,
        ICheckoutRepository checkoutRepository)
        {
            _userRepository = userRepository;
            _checkoutRepository = checkoutRepository;
        }

        // ✅ Obtener resumen previo al pago.
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetCheckout(
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

            var checkout =
            await _checkoutRepository
            .GetCheckoutAsync(
            orderId,
            user.Id);

            if (checkout == null)
            {
                return NotFound(
                "Pedido no encontrado.");
            }

            return Ok(checkout);
        }
    }
}