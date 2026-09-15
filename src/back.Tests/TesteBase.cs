using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace YourProject.Tests
{
    public abstract class TesteBase : IClassFixture<CashWiseApiFactory>
    {
        protected readonly HttpClient Client;

        protected TesteBase(CashWiseApiFactory factory)
        {
            Client = factory.CreateClient();
        }

        protected record UsuarioDeTeste(int UsuarioId, string Nome, string Email, string Senha, string Token);

        /// <summary>
        /// Cadastra e loga um usuário novo (email único a cada chamada), devolvendo
        /// os dados dele e o token já pronto para uso.
        /// </summary>
        protected async Task<UsuarioDeTeste> CriarUsuarioAutenticadoAsync(string nome = "Usuário de Teste")
        {
            var email = $"{Guid.NewGuid():N}@teste.com";
            const string senha = "senhaDeTeste123";

            var registroResponse = await Client.PostAsJsonAsync("/api/Auth/register", new
            {
                nome,
                email,
                senha
            });
            registroResponse.EnsureSuccessStatusCode();

            var loginResponse = await Client.PostAsJsonAsync("/api/Auth/login", new
            {
                email,
                senha
            });
            loginResponse.EnsureSuccessStatusCode();

            var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            return new UsuarioDeTeste(login!.UsuarioId, nome, email, senha, login.Token);
        }

        protected HttpClient ClienteAutenticadoComo(UsuarioDeTeste usuario)
        {
            var client = Client;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", usuario.Token);
            return client;
        }

        private class LoginResponseDto
        {
            public int UsuarioId { get; set; }
            public string Token { get; set; } = string.Empty;
        }
    }
}
