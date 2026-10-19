using System.ComponentModel.DataAnnotations;

namespace CashWiseAPI.Models
{
    public class MetaFinanceira : IValidatableObject
    {
        private static readonly HashSet<string> StatusPermitidos =
        ["Em andamento", "Quase lá", "Concluída"];

        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public double Valor { get; set; }

        public DateTime Prazo { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public int UsuarioId { get; set; }

        [Required]
        public Pessoa Usuario { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!StatusPermitidos.Contains(Status))
            {
                yield return new ValidationResult(
                    $"Status deve ser um destes valores: {string.Join(", ", StatusPermitidos)}.",
                    [nameof(Status)]);
            }

            if (Prazo == DateTime.MinValue)
            {
                yield return new ValidationResult(
                    "Prazo é obrigatório e não pode ser DateTime.MinValue.",
                    [nameof(Prazo)]);
            }
        }

        public void AddMeta() { }
        public void AtualizarMeta() { }
        public void VerificarProgresso() { }
    }
}
