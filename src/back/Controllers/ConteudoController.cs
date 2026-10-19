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
    public class ConteudoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConteudoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Conteudo — leitura pública, qualquer um pode ler os artigos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConteudoResponse>>> GetConteudos()
        {
            var conteudos = await _context.Conteudos
                .Include(c => c.Usuario)
                .OrderByDescending(c => c.DataPublicacao)
                .ToListAsync();

            return conteudos.Select(ToResponse).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConteudoResponse>> GetConteudo(int id)
        {
            var conteudo = await _context.Conteudos
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (conteudo == null)
            {
                return NotFound();
            }

            return ToResponse(conteudo);
        }

        // POST: api/Conteudo — exige login; o autor é sempre quem está autenticado
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ConteudoResponse>> CreateConteudo(ConteudoRequest request)
        {
            var usuarioId = User.GetUsuarioId();
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var conteudo = new Conteudo
            {
                Titulo = request.Titulo,
                Descricao = request.Descricao,
                Tipo = request.Tipo,
                Nivel = request.Nivel,
                DataPublicacao = DateTime.UtcNow,
                UsuarioFk = usuarioId,
                Usuario = usuario
            };

            _context.Conteudos.Add(conteudo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConteudo), new { id = conteudo.Id }, ToResponse(conteudo));
        }

        // PUT: api/Conteudo/5 — só o autor pode editar o próprio conteúdo
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConteudo(int id, ConteudoRequest request)
        {
            var usuarioId = User.GetUsuarioId();

            var conteudo = await _context.Conteudos
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioFk == usuarioId);

            if (conteudo == null)
            {
                return NotFound();
            }

            conteudo.Titulo = request.Titulo;
            conteudo.Descricao = request.Descricao;
            conteudo.Tipo = request.Tipo;
            conteudo.Nivel = request.Nivel;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Conteudo/5 — só o autor pode excluir o próprio conteúdo
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConteudo(int id)
        {
            var usuarioId = User.GetUsuarioId();

            var conteudo = await _context.Conteudos
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioFk == usuarioId);

            if (conteudo == null)
            {
                return NotFound();
            }

            _context.Conteudos.Remove(conteudo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static ConteudoResponse ToResponse(Conteudo c) => new()
        {
            Id = c.Id,
            Titulo = c.Titulo,
            Descricao = c.Descricao,
            Tipo = c.Tipo,
            Nivel = c.Nivel,
            DataPublicacao = c.DataPublicacao,
            AutorNome = c.Usuario?.Nome ?? string.Empty
        };
    }
}
