using System.Net.Http.Json;

namespace YourProject.Tests
{
    public class CategoriaTransacaoTests : TesteBase
    {
        public CategoriaTransacaoTests(CashWiseApiFactory factory) : base(factory) { }

        private record TransacaoDto(int Id, string Descricao, double Valor, string Tipo, string Categoria, DateTime Data);

        [Fact]
        public async Task CriarTransacao_SemCategoria_UsaOutrosComoPadrao()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Sem categoria",
                valor = 50,
                tipo = "Despesa"
            });

            var criada = await response.Content.ReadFromJsonAsync<TransacaoDto>();
            Assert.Equal("Outros", criada!.Categoria);
        }

        [Fact]
        public async Task CriarTransacao_ComCategoria_PreservaOValorEnviado()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.PostAsJsonAsync("/api/Transacao", new
            {
                descricao = "Supermercado",
                valor = 250,
                tipo = "Despesa",
                categoria = "Alimentação"
            });

            var criada = await response.Content.ReadFromJsonAsync<TransacaoDto>();
            Assert.Equal("Alimentação", criada!.Categoria);
        }
    }
}
