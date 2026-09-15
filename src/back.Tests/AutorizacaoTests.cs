using System.Net;
using System.Net.Http.Headers;

namespace YourProject.Tests
{
    public class AutorizacaoTests : TesteBase
    {
        public AutorizacaoTests(CashWiseApiFactory factory) : base(factory) { }

        [Theory]
        [InlineData("/api/Transacao")]
        [InlineData("/api/MetaFinanceira")]
        [InlineData("/api/Dashboard/resumo")]
        [InlineData("/api/Usuario/me")]
        public async Task EndpointFinanceiro_SemToken_Retorna401(string caminho)
        {
            Client.DefaultRequestHeaders.Authorization = null;

            var response = await Client.GetAsync(caminho);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task EndpointFinanceiro_ComTokenInvalido_Retorna401()
        {
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "token-forjado-invalido");

            var response = await Client.GetAsync("/api/Transacao");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task EndpointFinanceiro_ComTokenValido_Retorna200()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.GetAsync("/api/Transacao");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ConteudoPublico_LeituraSemToken_Retorna200()
        {
            Client.DefaultRequestHeaders.Authorization = null;

            var response = await Client.GetAsync("/api/Conteudo");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ConteudoPublico_EscritaSemToken_Retorna401()
        {
            Client.DefaultRequestHeaders.Authorization = null;

            var response = await Client.PostAsync("/api/Conteudo", null);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
