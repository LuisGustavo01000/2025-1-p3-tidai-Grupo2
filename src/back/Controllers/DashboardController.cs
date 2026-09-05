using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourProject.Data;
using YourProject.Dtos;
using YourProject.Extensions;

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

        // GET: api/Dashboard/resumo
        // Calculado sob demanda a partir das transações do usuário autenticado —
        // não existe mais uma tabela "Dashboard" guardando um snapshot manual
        // (ela existia, mas nunca era escrita por ninguém).
        [HttpGet("resumo")]
        public async Task<ActionResult<DashboardResumoResponse>> GetResumo()
        {
            var usuarioId = User.GetUsuarioId();

            var transacoes = await _context.Transacoes
                .Where(t => t.Usuario.Id == usuarioId)
                .ToListAsync();

            var agora = DateTime.UtcNow;

            var totalReceitas = transacoes
                .Where(t => t.Tipo.Equals("Receita", StringComparison.OrdinalIgnoreCase))
                .Sum(t => t.Valor);

            var totalDespesas = transacoes
                .Where(t => t.Tipo.Equals("Despesa", StringComparison.OrdinalIgnoreCase))
                .Sum(t => t.Valor);

            var gastosMes = transacoes
                .Where(t => t.Tipo.Equals("Despesa", StringComparison.OrdinalIgnoreCase)
                         && t.Data.Month == agora.Month
                         && t.Data.Year == agora.Year)
                .Sum(t => t.Valor);

            var metasAtivas = await _context.MetasFinanceiras
                .CountAsync(m => m.Usuario.Id == usuarioId);

            return new DashboardResumoResponse
            {
                SaldoTotal = totalReceitas - totalDespesas,
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                GastosMes = gastosMes,
                MetasAtivas = metasAtivas
            };
        }
    }
}
