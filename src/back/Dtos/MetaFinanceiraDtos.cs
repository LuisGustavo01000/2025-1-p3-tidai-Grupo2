using System.ComponentModel.DataAnnotations;

namespace YourProject.Dtos
{
    public class MetaFinanceiraRequest
    {
        [Required]
        [StringLength(56)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public double Valor { get; set; }

        [Required]
        public DateTime Prazo { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }

    public class MetaFinanceiraResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public double Valor { get; set; }
        public DateTime Prazo { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
