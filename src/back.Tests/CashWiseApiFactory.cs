using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using YourProject.Data;

namespace YourProject.Tests
{
    /// <summary>
    /// Sobe a API inteira em memória para os testes de integração, trocando o
    /// MySQL real por um banco EF InMemory isolado (um nome de banco novo por
    /// instância). Isso deixa os testes rápidos e sem depender do Docker/MySQL
    /// estarem de pé.
    /// </summary>
    public class CashWiseApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));
            });
        }
    }
}
