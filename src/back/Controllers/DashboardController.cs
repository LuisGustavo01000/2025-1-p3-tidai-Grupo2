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
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DashboardResponse>>> GetDashboards()
        {
            var usuarioId = User.GetUsuarioId();

            var dashboards = await _context.Dashboards
                .Where(d => d.Usuario.Id == usuarioId)
                .ToListAsync();

            return dashboards.Select(ToResponse).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DashboardResponse>> GetDashboard(int id)
        {
            var usuarioId = User.GetUsuarioId();

            var dashboard = await _context.Dashboards
                .FirstOrDefaultAsync(d => d.Id == id && d.Usuario.Id == usuarioId);

            if (dashboard == null)
            {
                return NotFound();
            }

            return ToResponse(dashboard);
        }

        [HttpPost]
        public async Task<ActionResult<DashboardResponse>> CreateDashboard(DashboardRequest request)
        {
            var usuarioId = User.GetUsuarioId();
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var dashboard = new Dashboard
            {
                SaldoTotal = request.SaldoTotal,
                InvestimentoTotal = request.InvestimentoTotal,
                Usuario = usuario
            };

            _context.Dashboards.Add(dashboard);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDashboard), new { id = dashboard.Id }, ToResponse(dashboard));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDashboard(int id, DashboardRequest request)
        {
            var usuarioId = User.GetUsuarioId();

            var dashboard = await _context.Dashboards
                .FirstOrDefaultAsync(d => d.Id == id && d.Usuario.Id == usuarioId);

            if (dashboard == null)
            {
                return NotFound();
            }

            dashboard.SaldoTotal = request.SaldoTotal;
            dashboard.InvestimentoTotal = request.InvestimentoTotal;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static DashboardResponse ToResponse(Dashboard d) => new()
        {
            Id = d.Id,
            SaldoTotal = d.SaldoTotal,
            InvestimentoTotal = d.InvestimentoTotal
        };
    }
}
