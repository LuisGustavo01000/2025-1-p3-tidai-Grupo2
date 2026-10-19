using System.Net;
using System.Net.Http.Json;

namespace YourProject.Tests
{
    public class DashboardTests : TesteBase
    {
        public DashboardTests(CashWiseApiFactory factory) : base(factory) { }

        private record DashboardResumoDto(
            double SaldoTotal,
            double TotalReceitas,
            double TotalDespesas,
            double GastosMes,
            int MetasAtivas);

        [Fact]
        public async Task Resumo_SemToken_Retorna401()
        {
            Client.DefaultRequestHeaders.Authorization = null;

            var response = await Client.GetAsync("/api/Dashboard/resumo");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Resumo_SemTransacoes_RetornaTudoZerado()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var resumo = await client.GetFromJsonAsync<DashboardResumoDto>("/api/Dashboard/resumo");

            Assert.Equal(0, resumo!.SaldoTotal);
            Assert.Equal(0, resumo.GastosMes);
            Assert.Equal(0, resumo.MetasAtivas);
        }

        [Fact]
        public async Task Resumo_CalculaSaldoComoReceitasMenosDespesas()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            await client.PostAsJsonAsync("/api/Transacao", new { descricao = "Salário", valor = 3000, tipo = "Receita" });
            await client.PostAsJsonAsync("/api/Transacao", new { descricao = "Aluguel", valor = 1200, tipo = "Despesa" });

            var resumo = await client.GetFromJsonAsync<DashboardResumoDto>("/api/Dashboard/resumo");

            Assert.Equal(3000, resumo!.TotalReceitas);
            Assert.Equal(1200, resumo.TotalDespesas);
            Assert.Equal(1800, resumo.SaldoTotal);
            Assert.Equal(1200, resumo.GastosMes); // criada agora -> conta no mês atual
        }

        [Fact]
        public async Task Resumo_ContaApenasMetasDoProprioUsuario()
        {
            var usuarioA = await CriarUsuarioAutenticadoAsync("Usuário A");
            var usuarioB = await CriarUsuarioAutenticadoAsync("Usuário B");

            var clienteA = ClienteAutenticadoComo(usuarioA);
            await clienteA.PostAsJsonAsync("/api/MetaFinanceira", new
            {
                nome = "Viagem",
                valor = 2000,
                prazo = "2027-01-01",
                status = "Em andamento"
            });

            var clienteB = ClienteAutenticadoComo(usuarioB);
            var resumoDeB = await clienteB.GetFromJsonAsync<DashboardResumoDto>("/api/Dashboard/resumo");

            Assert.Equal(0, resumoDeB!.MetasAtivas);
        }
    }
}
