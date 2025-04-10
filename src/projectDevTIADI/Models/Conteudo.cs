using System;
using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class Conteudo
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; } // "Receita" ou "Economia"
        public string Nivel { get; set; } // "Iniciante" ou "Avançado"

        public void GetId() { }
        public void GetConteudo() { }
        public void SetConteudo(string conteudo) { }
        public void GetDescricaoConteudo(string conteudo) { }
    }
}