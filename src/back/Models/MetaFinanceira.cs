using System;
using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class MetaFinanceira
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public double Valor { get; set; }
        public DateTime Prazo { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        public Usuario Usuario { get; set; } = null!;
    }
}
