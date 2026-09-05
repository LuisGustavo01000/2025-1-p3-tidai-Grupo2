using System.Net;
using System.Net.Http.Json;

namespace YourProject.Tests
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

        private record TransacaoDto(int Id, string Descricao, double Valor, string Tipo, DateTime Data);
    }
}
