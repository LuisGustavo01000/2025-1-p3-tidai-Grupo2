using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models
{
    [Table("USUARIO")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("NOME_USUARIO")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Column("EMAIL_USUARIO")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("SENHA_USUARIO")]
        public string Senha { get; set; } = string.Empty;

        [Column("ENDIVIDADO")]
        public bool Endividado { get; set; }

        [Column("DATA_CRIACAO")]
        public DateTime DataCriacao { get; set; }

        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}
