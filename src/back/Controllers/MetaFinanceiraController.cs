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
    public class MetaFinanceiraController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MetaFinanceiraController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetaFinanceiraResponse>>> GetMetasFinanceiras()
        {
            var usuarioId = User.GetUsuarioId();

            var metas = await _context.MetasFinanceiras
                .Where(m => m.Usuario.Id == usuarioId)
                .ToListAsync();

            return metas.Select(ToResponse).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MetaFinanceiraResponse>> GetMetaFinanceira(int id)
        {
            var usuarioId = User.GetUsuarioId();

            var meta = await _context.MetasFinanceiras
                .FirstOrDefaultAsync(m => m.Id == id && m.Usuario.Id == usuarioId);

            if (meta == null)
            {
                return NotFound();
            }

            return ToResponse(meta);
        }

        [HttpPost]
        public async Task<ActionResult<MetaFinanceiraResponse>> CreateMetaFinanceira(MetaFinanceiraRequest request)
        {
            var usuarioId = User.GetUsuarioId();
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var meta = new MetaFinanceira
            {
                Nome = request.Nome,
                Valor = request.Valor,
                Prazo = request.Prazo,
                Status = request.Status,
                Usuario = usuario
            };

            _context.MetasFinanceiras.Add(meta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMetaFinanceira), new { id = meta.Id }, ToResponse(meta));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMetaFinanceira(int id, MetaFinanceiraRequest request)
        {
            var usuarioId = User.GetUsuarioId();

            var meta = await _context.MetasFinanceiras
                .FirstOrDefaultAsync(m => m.Id == id && m.Usuario.Id == usuarioId);

            if (meta == null)
            {
                return NotFound();
            }

            meta.Nome = request.Nome;
            meta.Valor = request.Valor;
            meta.Prazo = request.Prazo;
            meta.Status = request.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMetaFinanceira(int id)
        {
            var usuarioId = User.GetUsuarioId();

            var meta = await _context.MetasFinanceiras
                .FirstOrDefaultAsync(m => m.Id == id && m.Usuario.Id == usuarioId);

            if (meta == null)
            {
                return NotFound();
            }

            _context.MetasFinanceiras.Remove(meta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static MetaFinanceiraResponse ToResponse(MetaFinanceira m) => new()
        {
            Id = m.Id,
            Nome = m.Nome,
            Valor = m.Valor,
            Prazo = m.Prazo,
            Status = m.Status
        };
    }
}
