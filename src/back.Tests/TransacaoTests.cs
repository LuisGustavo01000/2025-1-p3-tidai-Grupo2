using System.Net;
using System.Net.Http.Json;

namespace CashWiseAPI.Tests
{
    public class TransacaoTests : TesteBase
    {
        public TransacaoTests(CashWiseApiFactory factory) : base(factory) { }

        [Fact]
        public async Task CriarTransacao_VinculaAoUsuarioAutenticado_EApareceNaPropriaListagem()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var criar = await client.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Freelance",
                valor = 800,
                tipo = "Receita"
            });

            Assert.Equal(HttpStatusCode.Created, criar.StatusCode);

            var listagem = await client.GetFromJsonAsync<List<TransacaoDto>>("/api/Transacao");

            Assert.Single(listagem!);
            Assert.Equal("Freelance", listagem![0].Descricao);
        }

        [Fact]
        public async Task CriarTransacao_IgnoraQualquerUsuarioEnviadoNoCorpo()
        {
            // O DTO de request nem aceita um campo "usuario"/"usuarioId" — isso
            // é o que fecha o IDOR no POST. Aqui só confirmamos que a transação
            // criada pertence exclusivamente a quem está autenticado.
            var usuarioA = await CriarUsuarioAutenticadoAsync("Usuário A");
            var usuarioB = await CriarUsuarioAutenticadoAsync("Usuário B");

            var clienteA = ClienteAutenticadoComo(usuarioA);
            await clienteA.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Tentativa",
                valor = 1,
                tipo = "Receita",
                usuarioId = usuarioB.UsuarioId // campo deve ser ignorado pelo model binding
            });

            var clienteB = ClienteAutenticadoComo(usuarioB);
            var listaDeB = await clienteB.GetFromJsonAsync<List<TransacaoDto>>("/api/Transacao");

            Assert.Empty(listaDeB!);
        }

        [Fact]
        public async Task AtualizarTransacao_PutPermaneceOwnerScoped()
        {
            var usuarioA = await CriarUsuarioAutenticadoAsync("Usuário A");
            var usuarioB = await CriarUsuarioAutenticadoAsync("Usuário B");
            var clienteA = ClienteAutenticadoComo(usuarioA);

            var criada = await clienteA.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Original",
                valor = 100,
                tipo = "Receita"
            });
            var transacao = await criada.Content.ReadFromJsonAsync<TransacaoDto>();

            var clienteB = ClienteAutenticadoComo(usuarioB);
            var acessoDeB = await clienteB.PutAsJsonAsync($"/api/Transacao/{transacao!.Id}", new
            {
                descricao = "Alterada por terceiro",
                valor = 500,
                tipo = "Despesa"
            });

            Assert.Equal(HttpStatusCode.NotFound, acessoDeB.StatusCode);
            var atualizada = await clienteA.PutAsJsonAsync($"/api/Transacao/{transacao.Id}", new
            {
                descricao = "Atualizada",
                valor = 150,
                tipo = "Receita"
            });

            Assert.Equal(HttpStatusCode.OK, atualizada.StatusCode);
            var resposta = await atualizada.Content.ReadFromJsonAsync<TransacaoDto>();
            Assert.Equal("Atualizada", resposta!.Descricao);
        }

        [Fact]
        public async Task AtualizarTransacao_PatchAtualizaApenasCamposInformados()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);
            var criada = await client.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Original",
                valor = 100,
                tipo = "Receita",
                categoria = "Trabalho"
            });
            var transacao = await criada.Content.ReadFromJsonAsync<TransacaoDto>();

            var resposta = await client.PatchAsJsonAsync($"/api/Transacao/{transacao!.Id}", new
            {
                valor = 125
            });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var atualizada = await resposta.Content.ReadFromJsonAsync<TransacaoDto>();
            Assert.Equal("Original", atualizada!.Descricao);
            Assert.Equal(125, atualizada.Valor);
        }

        private record TransacaoDto(int Id, string Descricao, double Valor, string Tipo, DateTime Data);
    }
}
