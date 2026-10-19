using CashWiseAPI.Controllers;
using CashWiseAPI.Models;
using CashWiseAPI.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashWiseAPI.Tests;

public class TransacaoControllerTests
{
    [Fact]
    public async Task Put_Atualiza_Apenas_Transacao_Do_Proprio_Usuario()
    {
        await using var context = TestDbHelper.CreateContext();
        context.Pessoas.AddRange(
            new Pessoa { Id = 1, Nome = "U1", Email = "u1@email.com", Senha = "senha123" },
            new Pessoa { Id = 2, Nome = "U2", Email = "u2@email.com", Senha = "senha123" }
        );
        context.Transacoes.AddRange(
            new Transacao { Id = 10, Descricao = "Original", Tipo = "Receita", Valor = 100, Data = DateTime.UtcNow, UsuarioId = 1 },
            new Transacao { Id = 11, Descricao = "Outra", Tipo = "Despesa", Valor = 50, Data = DateTime.UtcNow, UsuarioId = 2 }
        );
        await context.SaveChangesAsync();

        var controller = CreateController(context, 1);

        var response = await controller.UpdateTransacao(11, new TransacaoUpdateRequest
        {
            Descricao = "Hack",
            Tipo = "Receita",
            Valor = 999,
            Data = DateTime.UtcNow
        });

        Assert.IsType<NotFoundResult>(response);

        var unchanged = await context.Transacoes.SingleAsync(t => t.Id == 11);
        Assert.Equal("Outra", unchanged.Descricao);
    }

    [Fact]
    public async Task Patch_Atualiza_Campos_Parciais_Do_Proprietario()
    {
        await using var context = TestDbHelper.CreateContext();
        context.Pessoas.Add(new Pessoa { Id = 1, Nome = "U1", Email = "u1@email.com", Senha = "senha123" });
        context.Transacoes.Add(new Transacao
        {
            Id = 15,
            Descricao = "Antes",
            Tipo = "Receita",
            Valor = 100,
            Data = new DateTime(2026, 1, 1),
            UsuarioId = 1
        });
        await context.SaveChangesAsync();

        var controller = CreateController(context, 1);

        var response = await controller.PatchTransacao(15, new TransacaoPatchRequest
        {
            Descricao = "Depois",
            Valor = 250
        });

        Assert.IsType<NoContentResult>(response);

        var updated = await context.Transacoes.SingleAsync(t => t.Id == 15);
        Assert.Equal("Depois", updated.Descricao);
        Assert.Equal(250, updated.Valor);
        Assert.Equal("Receita", updated.Tipo);
    }

    [Fact]
    public async Task Get_Retorna_Apenas_Transacoes_Do_Usuario_Logado()
    {
        await using var context = TestDbHelper.CreateContext();
        context.Pessoas.AddRange(
            new Pessoa { Id = 1, Nome = "U1", Email = "u1@email.com", Senha = "senha123" },
            new Pessoa { Id = 2, Nome = "U2", Email = "u2@email.com", Senha = "senha123" }
        );
        context.Transacoes.AddRange(
            new Transacao { Id = 1, Descricao = "A", Tipo = "Receita", Valor = 1, Data = DateTime.UtcNow, UsuarioId = 1 },
            new Transacao { Id = 2, Descricao = "B", Tipo = "Receita", Valor = 2, Data = DateTime.UtcNow, UsuarioId = 2 }
        );
        await context.SaveChangesAsync();

        var controller = CreateController(context, 1);

        var result = await controller.GetTransacoes();
        var transacoes = Assert.IsType<List<Transacao>>(result.Value);

        Assert.Single(transacoes);
        Assert.Equal(1, transacoes[0].UsuarioId);
    }

    private static TransacaoController CreateController(CashWiseAPI.Data.ApplicationDbContext context, int usuarioId)
    {
        var controller = new TransacaoController(context);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Usuario-Id"] = usuarioId.ToString();
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }
}
