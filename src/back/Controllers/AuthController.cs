using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YourProject.Data;
using YourProject.Dtos;
using YourProject.Models;

namespace YourProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request)
        {
            var emailJaExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == request.Email);

            if (emailJaExiste)
            {
                return Conflict(new { mensagem = "Já existe uma conta com este e-mail." });
            }

            var usuario = new Usuario
            {
                Nome = request.Nome,
                Email = request.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                Endividado = false
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var response = new RegisterResponse
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            };

            return CreatedAtAction(nameof(Register), new { id = usuario.Id }, response);
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
            {
                // Mesma mensagem para e-mail inexistente ou senha errada:
                // não dar pista de qual dos dois está incorreto.
                return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
            }

            var token = GerarToken(usuario, out var expiraEmUtc);

            return Ok(new AuthResponse
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Token = token,
                ExpiraEmUtc = expiraEmUtc
            });
        }

        private string GerarToken(Usuario usuario, out DateTime expiraEmUtc)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var expiresMinutes = jwtSection.GetValue<int>("ExpiresMinutes");
            expiraEmUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expiraEmUtc,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
