using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Senha { get; set; }

        public virtual ICollection<Transacao> Transacoes { get; set; }

        public bool Endividado { get; set; }

        public User()
        {
            Transacoes = new List<Transacao>();
        }
    }
}