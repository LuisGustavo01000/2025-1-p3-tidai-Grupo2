using CashWiseAPI.Controllers;
using CashWiseAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CashWiseAPI.Tests;

public class PasswordPolicyControllerTests
{
    [Fact]
    public async Task Pessoa_Create_Rejeita_Senha_Fraca()
    {
        await using var context = TestDbHelper.CreateContext();
        var controller = new PessoaController(context);

        var result = await controller.CreatePessoa(new Pessoa
        {
            Nome = "Pessoa",
            Email = "pessoa@email.com",
            Senha = "abcdefg"
        });

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task User_Update_Rejeita_Senha_Sem_Numero()
    {
        await using var context = TestDbHelper.CreateContext();
        context.Users.Add(new User { Id = 1, Nome = "User", Email = "u@email.com", Senha = "valida123" });
        await context.SaveChangesAsync();

        var controller = new UserController(context);

        var result = await controller.PutUser(1, new User
        {
            Id = 1,
            Nome = "User",
            Email = "u@email.com",
            Senha = "somenteletras"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Pessoa_Create_Aceita_Senha_Valida()
    {
        await using var context = TestDbHelper.CreateContext();
        var controller = new PessoaController(context);

        var result = await controller.CreatePessoa(new Pessoa
        {
            Nome = "Pessoa",
            Email = "pessoa@email.com",
            Senha = "senha1234"
        });

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }
}
