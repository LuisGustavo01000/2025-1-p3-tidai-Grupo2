using System.Net;
using System.Net.Http.Json;

namespace YourProject.Tests
{
    public class AuthTests : TesteBase
    {
        public AuthTests(CashWiseApiFactory factory) : base(factory) { }

        [Fact]
        public async Task Register_ComDadosValidos_Retorna201ESemSenhaNaResposta()
        {
            var email = $"{Guid.NewGuid():N}@teste.com";

            var response = await Client.PostAsJsonAsync("/api/Auth/register", new
            {
                nome = "Fulano",
                email,
                senha = "senhaValida123"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var corpo = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain("senhaValida123", corpo, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("\"senha\"", corpo, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Register_ComEmailJaCadastrado_Retorna409()
        {
            var email = $"{Guid.NewGuid():N}@teste.com";
            var payload = new { nome = "Fulano", email, senha = "senhaValida123" };

            await Client.PostAsJsonAsync("/api/Auth/register", payload);
            var segundaTentativa = await Client.PostAsJsonAsync("/api/Auth/register", payload);

            Assert.Equal(HttpStatusCode.Conflict, segundaTentativa.StatusCode);
        }

        [Theory]
        [InlineData("1234567")]   // 7 caracteres, abaixo do mínimo de 8
        [InlineData("")]
        public async Task Register_ComSenhaInvalida_Retorna400(string senha)
        {
            var response = await Client.PostAsJsonAsync("/api/Auth/register", new
            {
                nome = "Fulano",
                email = $"{Guid.NewGuid():N}@teste.com",
                senha
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_ComCredenciaisCorretas_RetornaToken()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();

            Assert.False(string.IsNullOrWhiteSpace(usuario.Token));
        }

        [Fact]
        public async Task Login_ComSenhaErrada_Retorna401()
        {
            var usuario = await CriarUsuarioAutenticadoAsync();

            var response = await Client.PostAsJsonAsync("/api/Auth/login", new
            {
                email = usuario.Email,
                senha = "senhaErrada"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ComEmailInexistente_Retorna401()
        {
            var response = await Client.PostAsJsonAsync("/api/Auth/login", new
            {
                email = "ninguem@nunca-cadastrado.com",
                senha = "qualquerSenha123"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
