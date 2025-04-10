using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class Pessoa
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public List<Transacao> Transacoes { get; set; }
        public bool Endividado { get; set; }

        public Pessoa()
        {
            Transacoes = new List<Transacao>();
        }

        public void Cadastrar() { }
        public void Login() { }
        public void GetId() { }
        public void GetNome() { }
        public void GetEmail() { }
        public void GetSenha() { }
        public void SetNome(string nome) { }
        public void SetEmail(string email) { }
        public void SetSenha(string senha) { }
        public void GetTransacoes() { }
        public void SetTransacoes(List<Transacao> transacoes) { }
    }
}