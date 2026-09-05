using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class Dashboard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Usuario Usuario { get; set; } = null!;

        public double SaldoTotal { get; set; }
        public double InvestimentoTotal { get; set; }
    }
}
