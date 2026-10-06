using CafeApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeApi.Controllers
{
    // ✅ Controlador para operaciones relacionadas
    // con el usuario autenticado.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        // ✅ Repositorio de usuarios.
        private readonly IUserRepository _userRepository;

        // ✅ Constructor.
        public UsersController(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // ✅ Obtiene información del usuario
        // autenticado mediante JWT.
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            // ✅ Obtener email desde el token JWT.
            var email =
                User.FindFirstValue(
                    ClaimTypes.Email
                );

            // ✅ Validar email.
            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(
                    "No fue posible obtener el correo del token."
                );
            }

            // ✅ Buscar usuario.
            var user =
                await _userRepository
                    .GetByEmailAsync(email);

            // ✅ Usuario no encontrado.
            if (user == null)
            {
                return NotFound(
                    "Usuario no encontrado."
                );
            }

            // ✅ Retornar información.
            return Ok(user);
        }
    }
}