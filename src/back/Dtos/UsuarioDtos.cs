using System.ComponentModel.DataAnnotations;

namespace YourProject.Dtos
{
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Endividado { get; set; }
    }

    public class UpdateUsuarioRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
    }

    public class AlterarSenhaRequest
    {
        [Required]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NovaSenha { get; set; } = string.Empty;
    }
}
