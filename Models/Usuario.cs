using System.ComponentModel.DataAnnotations;

namespace Dsin_CabeleleiraleiraAPI.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        public string? Token { get; set; }

        public string TipoUsuario { get; set; } = "cliente";

        public List<Agendamento>? Agendamentos { get; set; }
    }
}
