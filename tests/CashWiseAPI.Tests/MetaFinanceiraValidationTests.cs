using System.ComponentModel.DataAnnotations;
using CashWiseAPI.Models;

namespace CashWiseAPI.Tests;

public class MetaFinanceiraValidationTests
{
    [Fact]
    public void Status_Invalido_Deve_Falhar_Validacao()
    {
        var meta = new MetaFinanceira
        {
            Nome = "Meta",
            Valor = 100,
            Prazo = DateTime.UtcNow.AddDays(30),
            Status = "INVALIDO",
            Usuario = new Pessoa { Nome = "U", Email = "u@email.com", Senha = "senha123" }
        };

        var results = Validate(meta);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(MetaFinanceira.Status)));
    }

    [Fact]
    public void Prazo_MinValue_Deve_Falhar_Validacao()
    {
        var meta = new MetaFinanceira
        {
            Nome = "Meta",
            Valor = 100,
            Prazo = DateTime.MinValue,
            Status = "Em andamento",
            Usuario = new Pessoa { Nome = "U", Email = "u@email.com", Senha = "senha123" }
        };

        var results = Validate(meta);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(MetaFinanceira.Prazo)));
    }

    private static List<ValidationResult> Validate(object model)
    {
        var validationContext = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, validationContext, results, validateAllProperties: true);
        return results;
    }
}
