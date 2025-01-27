using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dsin_CabeleleiraleiraAPI.Models
{
    public class Agendamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        [Required]
        public string Servico { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
