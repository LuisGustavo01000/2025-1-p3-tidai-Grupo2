using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourProject.Data;
using YourProject.Dtos;
using YourProject.Extensions;
using YourProject.Models;

namespace YourProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuario/me — dados do próprio usuário autenticado, nunca de outro
        [HttpGet("me")]
        public async Task<ActionResult<UsuarioResponse>> GetMe()
        {
            var usuarioId = User.GetUsuarioId();
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            return ToResponse(usuario);
        }

        // PUT: api/Usuario/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(UpdateUsuarioRequest request)
        {
            var usuarioId = User.GetUsuarioId();
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            var emailEmUsoPorOutraConta = await _context.Usuarios
                .AnyAsync(u => u.Email == request.Email && u.Id != usuarioId);

            if (emailEmUsoPorOutraConta)
            {
                return Conflict(new { mensagem = "Já existe uma conta com este e-mail." });
            }

            usuario.Nome = request.Nome;
            usuario.Email = request.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Usuario/me/senha
        [HttpPut("me/senha")]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaRequest request)
        {
            var usuarioId = User.GetUsuarioId();
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(request.SenhaAtual, usuario.Senha))
            {
                return Unauthorized(new { mensagem = "Senha atual incorreta." });
            }

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Usuario/me
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            var usuarioId = User.GetUsuarioId();
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static UsuarioResponse ToResponse(Usuario u) => new()
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Endividado = u.Endividado
        };
    }
}
