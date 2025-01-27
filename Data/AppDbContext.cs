using Microsoft.EntityFrameworkCore; 
using Dsin_CabeleleiraleiraAPI.Models;

namespace Dsin_CabeleleiraleiraAPI.Data
{
    // CLASSE QUE REPRESENTA O CONTEXTO DO BANCO DE DADOS
    public class AppDbContext : DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet REPRESENTA AS TABELAS NO BANCO DE DADOS E PERMITE OPERAR SOBRE OS REGISTROS.
        public DbSet<Usuario> Usuarios { get; set; } // MAPEA A TABELA "Usuarios" COM BASE NO MODELO Usuario.
        public DbSet<Agendamento> Agendamentos { get; set; } // MAPEA A TABELA "Agendamentos" COM BASE NO MODELO Agendamento.
    }
}
