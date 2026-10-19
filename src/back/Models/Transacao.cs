using System;
using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class Transacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Descricao { get; set; } = string.Empty;

        public double Valor { get; set; }

        [Required]
        public string Tipo { get; set; } = string.Empty; // "Receita" ou "Despesa"

        [Required]
        public string Categoria { get; set; } = "Outros";

        public DateTime Data { get; set; }

        [Required]
        public Usuario Usuario { get; set; } = null!;
    }
}
