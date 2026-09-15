using System.Net;
using System.Net.Http.Json;

namespace YourProject.Tests
{
    public class UsuarioTests : TesteBase
    {
        public UsuarioTests(CashWiseApiFactory factory) : base(factory) { }

        [Fact]
        public async Task GetMe_NuncaExpoeSenha()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.GetAsync("/api/Usuario/me");
            var corpo = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.DoesNotContain("senha", corpo, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateMe_ComEmailJaUsadoPorOutraConta_Retorna409()
        {
            var usuarioA = await CriarUsuarioAutenticadoAsync("Usuário A");
            var usuarioB = await CriarUsuarioAutenticadoAsync("Usuário B");

            var clienteA = ClienteAutenticadoComo(usuarioA);
            var response = await clienteA.PutAsJsonAsync("/api/Usuario/me", new
            {
                nome = "Usuário A Renomeado",
                email = usuarioB.Email
            });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task AlterarSenha_ComSenhaAtualErrada_Retorna401()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var response = await client.PutAsJsonAsync("/api/Usuario/me/senha", new
            {
                senhaAtual = "senhaErrada",
                novaSenha = "novaSenha123"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AlterarSenha_ComSenhaAtualCorreta_PermiteLoginComNovaSenha()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();
            var client = ClienteAutenticadoComo(usuario);

            var alterar = await client.PutAsJsonAsync("/api/Usuario/me/senha", new
            {
                senhaAtual = usuario.Senha,
                novaSenha = "senhaNova456"
            });
            Assert.Equal(HttpStatusCode.NoContent, alterar.StatusCode);

            var loginComSenhaAntiga = await Client.PostAsJsonAsync("/api/Auth/login", new
            {
                email = usuario.Email,
                senha = usuario.Senha
            });
            Assert.Equal(HttpStatusCode.Unauthorized, loginComSenhaAntiga.StatusCode);

            var loginComSenhaNova = await Client.PostAsJsonAsync("/api/Auth/login", new
            {
                email = usuario.Email,
                senha = "senhaNova456"
            });
            Assert.Equal(HttpStatusCode.OK, loginComSenhaNova.StatusCode);
        }
    }
}
