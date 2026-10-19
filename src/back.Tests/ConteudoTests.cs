using System.Net;
using System.Net.Http.Json;

namespace YourProject.Tests
{
    /// <summary>
    /// Regressão para o achado 3/4 da auditoria de 2026-09-06: o dono do
    /// conteúdo vinha do corpo da requisição (nunca do token) e qualquer
    /// usuário autenticado podia editar/excluir conteúdo de outro.
    /// </summary>
    public class ConteudoTests : TesteBase
    {
        public ConteudoTests(CashWiseApiFactory factory) : base(factory) { }

        [Fact]
        public async Task LeituraPublica_SemToken_Retorna200()
        {
            Client.DefaultRequestHeaders.Authorization = null;

            var response = await Client.GetAsync("/api/Conteudo");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CriarConteudo_SemToken_Retorna401()
        {
            Client.DefaultRequestHeaders.Authorization = null;

            var response = await Client.PostAsJsonAsync("/api/Conteudo", new
            {
                titulo = "Título",
                descricao = "Descrição",
                tipo = "Economia",
                nivel = "Iniciante"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CriarConteudo_AutorEhSempreOUsuarioAutenticado()
        {
            var usuario = await CriarUsuarioAutenticadoAsync("Autor Real");
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.PostAsJsonAsync("/api/Conteudo", new
            {
                titulo = "Como economizar",
                descricao = "Dicas práticas",
                tipo = "Economia",
                nivel = "Iniciante"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var corpo = await response.Content.ReadFromJsonAsync<ConteudoDto>();
            Assert.Equal("Autor Real", corpo!.AutorNome);
        }

        [Fact]
        public async Task CriarConteudo_ComTipoInvalido_Retorna400()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.PostAsJsonAsync("/api/Conteudo", new
            {
                titulo = "Título",
                descricao = "Descrição",
                tipo = "Criptomoedas",
                nivel = "Iniciante"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UsuarioB_NaoConsegueEditarConteudoDeUsuarioA()
        {
            var usuarioA = await CriarUsuarioAutenticadoAsync("Usuário A");
            var usuarioB = await CriarUsuarioAutenticadoAsync("Usuário B");

            var clienteA = ClienteAutenticadoComo(usuarioA);
            var criado = await clienteA.PostAsJsonAsync("/api/Conteudo", new
            {
                titulo = "Artigo de A",
                descricao = "Conteúdo original",
                tipo = "Investimento",
                nivel = "Avançado"
            });
            var conteudo = await criado.Content.ReadFromJsonAsync<ConteudoDto>();

            var clienteB = ClienteAutenticadoComo(usuarioB);
            var tentativaEdicao = await clienteB.PutAsJsonAsync($"/api/Conteudo/{conteudo!.Id}", new
            {
                titulo = "Editado por B",
                descricao = "Invasão",
                tipo = "Investimento",
                nivel = "Avançado"
            });
            Assert.Equal(HttpStatusCode.NotFound, tentativaEdicao.StatusCode);

            var tentativaExclusao = await clienteB.DeleteAsync($"/api/Conteudo/{conteudo.Id}");
            Assert.Equal(HttpStatusCode.NotFound, tentativaExclusao.StatusCode);

            var aindaExiste = await Client.GetAsync($"/api/Conteudo/{conteudo.Id}");
            Assert.Equal(HttpStatusCode.OK, aindaExiste.StatusCode);
        }

        private record ConteudoDto(int Id, string Titulo, string AutorNome);
    }
}
