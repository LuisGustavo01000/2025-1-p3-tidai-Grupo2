using CashWiseAPI.Data;
using CashWiseAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashWiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dashboard>>> GetDashboards()
        {
            return await _context.Dashboards.Include(d => d.Usuario).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dashboard>> GetDashboard(int id)
        {
            var dashboard = await _context.Dashboards
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dashboard == null)
            {
                return NotFound();
            }

            return dashboard;
        }

        [HttpGet("resumo/{usuarioId:int}")]
        public async Task<ActionResult<DashboardResumoResponse>> GetResumo(int usuarioId, [FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var inicioPeriodo = inicio ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var fimPeriodo = fim ?? inicioPeriodo.AddMonths(1).AddTicks(-1);

            var receitas = await _context.Transacoes
                .Where(t => t.UsuarioId == usuarioId && t.Tipo == "Receita" && t.Data >= inicioPeriodo && t.Data <= fimPeriodo)
                .SumAsync(t => (double?)t.Valor) ?? 0;

            var despesas = await _context.Transacoes
                .Where(t => t.UsuarioId == usuarioId && t.Tipo == "Despesa" && t.Data >= inicioPeriodo && t.Data <= fimPeriodo)
                .SumAsync(t => (double?)t.Valor) ?? 0;

            var metasAtivas = await _context.MetasFinanceiras
                .Where(m => m.UsuarioId == usuarioId && (m.Status == "Em andamento" || m.Status == "Quase lá"))
                .CountAsync();

            return Ok(new DashboardResumoResponse
            {
                UsuarioId = usuarioId,
                Inicio = inicioPeriodo,
                Fim = fimPeriodo,
                TotalReceitas = receitas,
                TotalDespesas = despesas,
                Saldo = receitas - despesas,
                MetasAtivas = metasAtivas
            });
        }

        [HttpPost]
        public async Task<ActionResult<Dashboard>> CreateDashboard(Dashboard dashboard)
        {
            _context.Dashboards.Add(dashboard);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDashboard), new { id = dashboard.Id }, dashboard);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDashboard(int id, Dashboard dashboard)
        {
            if (id != dashboard.Id)
            {
                return BadRequest();
            }

            _context.Entry(dashboard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DashboardExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDashboard(int id)
        {
            var dashboard = await _context.Dashboards.FindAsync(id);
            if (dashboard == null)
            {
                return NotFound();
            }

            _context.Dashboards.Remove(dashboard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DashboardExists(int id)
        {
            return _context.Dashboards.Any(e => e.Id == id);
        }
    }

    public class DashboardResumoResponse
    {
        public int UsuarioId { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public double TotalReceitas { get; set; }
        public double TotalDespesas { get; set; }
        public double Saldo { get; set; }
        public int MetasAtivas { get; set; }
    }
}
