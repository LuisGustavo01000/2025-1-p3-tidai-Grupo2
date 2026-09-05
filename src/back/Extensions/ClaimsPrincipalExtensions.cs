using System.Security.Claims;

namespace YourProject.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUsuarioId(this ClaimsPrincipal user)
        {
            var valor = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("Token autenticado sem claim de usuário.");

            return int.Parse(valor);
        }
    }
}
