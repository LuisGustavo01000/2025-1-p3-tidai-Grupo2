using System.ComponentModel.DataAnnotations;

namespace YourProject.Dtos
{
    public class TransacaoRequest
    {
        [Required]
        public string Descricao { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public double Valor { get; set; }

        [Required]
        [RegularExpression("(?i:^(Receita|Despesa)$)", ErrorMessage = "Tipo deve ser 'Receita' ou 'Despesa'.")]
        public string Tipo { get; set; } = string.Empty;

        [StringLength(30)]
        public string? Categoria { get; set; }

        public DateTime? Data { get; set; }
    }

    public class TransacaoResponse
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public double Valor { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime Data { get; set; }
    }
}
