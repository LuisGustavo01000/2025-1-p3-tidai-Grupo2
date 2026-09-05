using System.ComponentModel.DataAnnotations;

namespace YourProject.Dtos
{
    public class TransacaoRequest
    {
        [Required]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public double Valor { get; set; }

        [Required]
        [StringLength(20)]
        public string Tipo { get; set; } = string.Empty; // "Receita" ou "Despesa"

        public DateTime? Data { get; set; }
    }

    public class TransacaoResponse
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public double Valor { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public DateTime Data { get; set; }
    }
}
