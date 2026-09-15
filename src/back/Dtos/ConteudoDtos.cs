using System.ComponentModel.DataAnnotations;

namespace YourProject.Dtos
{
    public class ConteudoRequest
    {
        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [RegularExpression("(?i:^(Investimento|Economia|Valorização)$)",
            ErrorMessage = "Tipo deve ser 'Investimento', 'Economia' ou 'Valorização'.")]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        [RegularExpression("(?i:^(Iniciante|Intermediário|Avançado)$)",
            ErrorMessage = "Nível deve ser 'Iniciante', 'Intermediário' ou 'Avançado'.")]
        public string Nivel { get; set; } = string.Empty;
    }

    public class ConteudoResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public DateTime DataPublicacao { get; set; }
        public string AutorNome { get; set; } = string.Empty;
    }
}
