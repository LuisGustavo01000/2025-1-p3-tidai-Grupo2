using System.Net;
using System.Net.Http.Json;

namespace YourProject.Tests
{
    /// <summary>
    /// Regressão para os achados 1 e 2 da auditoria de 2026-09-06:
    /// valores não-positivos e "Tipo" fora de Receita/Despesa eram aceitos
    /// silenciosamente e corrompiam o cálculo do dashboard.
    /// </summary>
    public class ValidacaoTransacaoTests : TesteBase
    {
        public ValidacaoTransacaoTests(CashWiseApiFactory factory) : base(factory) { }

        [Theory]
        [InlineData(-999)]
        [InlineData(0)]
        public async Task CriarTransacao_ComValorNaoPositivo_Retorna400(double valor)
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Transação inválida",
                valor,
                tipo = "Receita"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CriarTransacao_ComTipoForaDeReceitaOuDespesa_Retorna400()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Transação com tipo inválido",
                valor = 100,
                tipo = "Investimento"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("receita")]
        [InlineData("DESPESA")]
        public async Task CriarTransacao_ComTipoEmQualquerCaixa_Aceita(string tipo)
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Transação válida",
                valor = 100,
                tipo
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CriarMetaFinanceira_ComValorNaoPositivo_Retorna400()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.PostAsJsonAsync("/api/MetaFinanceira", new
            {
                nome = "Meta inválida",
                valor = -1,
                prazo = "2027-01-01",
                status = "Em andamento"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
