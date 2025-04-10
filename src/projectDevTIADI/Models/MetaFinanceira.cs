using System;
using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class MetaFinanceira
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public double Valor { get; set; }
        public DateTime Prazo { get; set; }
        public string Status { get; set; }
        public Pessoa Usuario { get; set; }

        public void AddMeta() { }
        public void AtualizarMeta() { }
        public void VerificarProgresso() { }
    }
}