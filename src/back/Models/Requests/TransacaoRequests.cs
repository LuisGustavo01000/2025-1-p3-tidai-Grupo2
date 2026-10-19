using System.ComponentModel.DataAnnotations;

namespace CashWiseAPI.Models.Requests;

public class TransacaoCreateRequest
{
    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [RegularExpression("Receita|Despesa")]
    public string Tipo { get; set; } = string.Empty;

    public double Valor { get; set; }

    public DateTime Data { get; set; }
}

public class TransacaoUpdateRequest : TransacaoCreateRequest
{
}

public class TransacaoPatchRequest
{
    public string? Descricao { get; set; }
    public string? Tipo { get; set; }
    public double? Valor { get; set; }
    public DateTime? Data { get; set; }
}
