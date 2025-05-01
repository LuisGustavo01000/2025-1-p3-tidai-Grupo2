using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models
{
    [Table("CONTEUDO")] 
    public class Conteudo
    {
        [Key]
        [Column("ID_CONTEUDO")]
        public int Id { get; set; }

        [Required]
        [Column("TITULO_CONTEUDO")]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [Column("DESCRICAO_CONTEUDO")]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Column("TIPO_CONTEUDO")]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        [Column("NIVEL_CONTEUDO")]
        public string Nivel { get; set; } = string.Empty;

        [Column("DATA_PUBLICACAO")]
        public DateTime DataPublicacao { get; set; }

        [Column("USUARIOFK")]
        public int UsuarioFk { get; set; }
    


    

        public void GetId() { }
        public void GetConteudo() { }
        public void SetConteudo(string conteudo) { }
        public void GetDescricaoConteudo(string conteudo) { }
    }
}