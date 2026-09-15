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
    public class TransacaoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransacaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Transacao (somente as transações do usuário autenticado)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransacaoResponse>>> GetTransacoes()
        {
            var usuarioId = User.GetUsuarioId();

            var transacoes = await _context.Transacoes
                .Where(t => t.Usuario.Id == usuarioId)
                .OrderByDescending(t => t.Data)
                .ToListAsync();

            return transacoes.Select(ToResponse).ToList();
        }

        // GET: api/Transacao/5 (só se pertencer ao usuário autenticado)
        [HttpGet("{id}")]
        public async Task<ActionResult<TransacaoResponse>> GetTransacao(int id)
        {
            var usuarioId = User.GetUsuarioId();

            var transacao = await _context.Transacoes
                .FirstOrDefaultAsync(t => t.Id == id && t.Usuario.Id == usuarioId);

            if (transacao == null)
            {
                return NotFound();
            }

            return ToResponse(transacao);
        }

        // POST: api/Transacao (o dono é sempre o usuário do token, nunca um valor vindo do corpo)
        [HttpPost]
        public async Task<ActionResult<TransacaoResponse>> CreateTransacao(TransacaoRequest request)
        {
            var usuarioId = User.GetUsuarioId();
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var transacao = new Transacao
            {
                Descricao = request.Descricao,
                Valor = request.Valor,
                Tipo = request.Tipo,
                Categoria = string.IsNullOrWhiteSpace(request.Categoria) ? "Outros" : request.Categoria,
                Data = request.Data ?? DateTime.UtcNow,
                Usuario = usuario
            };

            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransacao), new { id = transacao.Id }, ToResponse(transacao));
        }

        // DELETE: api/Transacao/5 (só se pertencer ao usuário autenticado)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransacao(int id)
        {
            var usuarioId = User.GetUsuarioId();

            var transacao = await _context.Transacoes
                .FirstOrDefaultAsync(t => t.Id == id && t.Usuario.Id == usuarioId);

            if (transacao == null)
            {
                return NotFound();
            }

            _context.Transacoes.Remove(transacao);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static TransacaoResponse ToResponse(Transacao t) => new()
        {
            Id = t.Id,
            Descricao = t.Descricao,
            Valor = t.Valor,
            Tipo = t.Tipo,
            Categoria = t.Categoria,
            Data = t.Data
        };
    }
}
