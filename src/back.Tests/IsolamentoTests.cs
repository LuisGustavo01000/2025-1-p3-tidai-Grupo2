using System.Net;
using System.Net.Http.Json;

namespace YourProject.Tests
{
    public class IsolamentoTests : TesteBase
    {
        public IsolamentoTests(CashWiseApiFactory factory) : base(factory) { }

        [Fact]
        public async Task UsuarioB_NaoVeTransacaoDeUsuarioA_NaListagem()
        {
            var usuarioA = await CriarUsuarioAutenticadoAsync("Usuário A");
            var usuarioB = await CriarUsuarioAutenticadoAsync("Usuário B");

            var clienteA = ClienteAutenticadoComo(usuarioA);
            await clienteA.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Salário",
                valor = 5000,
                tipo = "Receita"
            });

            var clienteB = ClienteAutenticadoComo(usuarioB);
            var listaDeB = await clienteB.GetFromJsonAsync<List<object>>("/api/Transacao");

            Assert.Empty(listaDeB!);
        }

        [Fact]
        public async Task UsuarioB_NaoConsegueLerTransacaoDeUsuarioAPeloId()
        {
            var usuarioA = await CriarUsuarioAutenticadoAsync("Usuário A");
            var usuarioB = await CriarUsuarioAutenticadoAsync("Usuário B");

            var clienteA = ClienteAutenticadoComo(usuarioA);
            var criada = await clienteA.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Aluguel",
                valor = 1200,
                tipo = "Despesa"
            });
            var transacaoCriada = await criada.Content.ReadFromJsonAsync<TransacaoDto>();

            var clienteB = ClienteAutenticadoComo(usuarioB);
            var resposta = await clienteB.GetAsync($"/api/Transacao/{transacaoCriada!.Id}");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task UsuarioB_NaoConsegueExcluirTransacaoDeUsuarioA()
        {
            var usuarioA = await CriarUsuarioAutenticadoAsync("Usuário A");
            var usuarioB = await CriarUsuarioAutenticadoAsync("Usuário B");

            var clienteA = ClienteAutenticadoComo(usuarioA);
            var criada = await clienteA.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Mercado",
                valor = 350,
                tipo = "Despesa"
            });
            var transacaoCriada = await criada.Content.ReadFromJsonAsync<TransacaoDto>();

            var clienteB = ClienteAutenticadoComo(usuarioB);
            var tentativaDeExclusao = await clienteB.DeleteAsync($"/api/Transacao/{transacaoCriada!.Id}");
            Assert.Equal(HttpStatusCode.NotFound, tentativaDeExclusao.StatusCode);

            // confirma que a transação de A continua existindo — B não conseguiu apagar
            var clienteAOutraVez = ClienteAutenticadoComo(usuarioA);
            var confirmacao = await clienteAOutraVez.GetAsync($"/api/Transacao/{transacaoCriada.Id}");
            Assert.Equal(HttpStatusCode.OK, confirmacao.StatusCode);
        }

        [Fact]
        public async Task UsuarioB_NaoVeMetaFinanceiraDeUsuarioA_MesmoComNomeIgual()
        {
            var usuarioA = await CriarUsuarioAutenticadoAsync("Usuário A");
            var usuarioB = await CriarUsuarioAutenticadoAsync("Usuário B");

            var clienteA = ClienteAutenticadoComo(usuarioA);
            await clienteA.PostAsJsonAsync("/api/MetaFinanceira", new
            {
                nome = "Viagem",
                valor = 3000,
                prazo = "2027-01-01",
                status = "Em andamento"
            });

            var clienteB = ClienteAutenticadoComo(usuarioB);
            var criadaPorB = await clienteB.PostAsJsonAsync("/api/MetaFinanceira", new
            {
                nome = "Viagem",
                valor = 9999,
                prazo = "2027-06-01",
                status = "Em andamento"
            });

            // nomes repetidos entre usuários diferentes não devem colidir (bug do db.sql corrigido)
            Assert.Equal(HttpStatusCode.Created, criadaPorB.StatusCode);

            var listaDeB = await clienteB.GetFromJsonAsync<List<MetaFinanceiraDto>>("/api/MetaFinanceira");
            Assert.Single(listaDeB!);
            Assert.Equal(9999, listaDeB![0].Valor);
        }

        private record TransacaoDto(int Id, string Descricao, double Valor, string Tipo, DateTime Data);
        private record MetaFinanceiraDto(int Id, string Nome, double Valor, DateTime Prazo, string Status);
    }
}
