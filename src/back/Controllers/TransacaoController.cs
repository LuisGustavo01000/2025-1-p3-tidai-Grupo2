using CashWiseAPI.Data;
using CashWiseAPI.Models;
using CashWiseAPI.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashWiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacaoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransacaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transacao>>> GetTransacoes()
        {
            if (!TryGetUsuarioId(out var usuarioId, out var badRequest))
            {
                return badRequest;
            }

            return await _context.Transacoes
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transacao>> GetTransacao(int id)
        {
            if (!TryGetUsuarioId(out var usuarioId, out var badRequest))
            {
                return badRequest;
            }

            var transacao = await _context.Transacoes
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transacao == null)
            {
                return NotFound();
            }

            return transacao;
        }

        [HttpPost]
        public async Task<ActionResult<Transacao>> CreateTransacao(TransacaoCreateRequest request)
        {
            if (!TryGetUsuarioId(out var usuarioId, out var badRequest))
            {
                return badRequest;
            }

            if (!await _context.Pessoas.AnyAsync(p => p.Id == usuarioId))
            {
                return BadRequest("Usuário não encontrado.");
            }

            var transacao = new Transacao
            {
                Descricao = request.Descricao,
                Valor = request.Valor,
                Tipo = request.Tipo,
                Data = request.Data == default ? DateTime.UtcNow : request.Data,
                UsuarioId = usuarioId
            };

            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransacao), new { id = transacao.Id }, transacao);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransacao(int id, TransacaoUpdateRequest request)
        {
            if (!TryGetUsuarioId(out var usuarioId, out var badRequest))
            {
                return badRequest;
            }

            var transacao = await _context.Transacoes
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transacao == null)
            {
                return NotFound();
            }

            transacao.Descricao = request.Descricao;
            transacao.Tipo = request.Tipo;
            transacao.Valor = request.Valor;
            transacao.Data = request.Data == default ? transacao.Data : request.Data;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTransacao(int id, TransacaoPatchRequest request)
        {
            if (!TryGetUsuarioId(out var usuarioId, out var badRequest))
            {
                return badRequest;
            }

            var transacao = await _context.Transacoes
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transacao == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(request.Descricao))
            {
                transacao.Descricao = request.Descricao;
            }

            if (!string.IsNullOrWhiteSpace(request.Tipo))
            {
                if (request.Tipo is not ("Receita" or "Despesa"))
                {
                    return BadRequest("Tipo inválido. Use Receita ou Despesa.");
                }

                transacao.Tipo = request.Tipo;
            }

            if (request.Valor.HasValue)
            {
                transacao.Valor = request.Valor.Value;
            }

            if (request.Data.HasValue)
            {
                transacao.Data = request.Data.Value;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransacao(int id)
        {
            if (!TryGetUsuarioId(out var usuarioId, out var badRequest))
            {
                return badRequest;
            }

            var transacao = await _context.Transacoes
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transacao == null)
            {
                return NotFound();
            }

            _context.Transacoes.Remove(transacao);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TryGetUsuarioId(out int usuarioId, out ActionResult badRequest)
        {
            usuarioId = 0;
            badRequest = BadRequest("Cabeçalho X-Usuario-Id é obrigatório.");

            if (!Request.Headers.TryGetValue("X-Usuario-Id", out var usuarioIdHeader) ||
                !int.TryParse(usuarioIdHeader, out usuarioId) || usuarioId <= 0)
            {
                return false;
            }

            badRequest = Ok();
            return true;
        }
    }
}
