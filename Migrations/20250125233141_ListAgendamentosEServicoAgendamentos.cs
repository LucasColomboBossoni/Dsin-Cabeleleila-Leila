using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dsin_CabeleleiraleiraAPI.Migrations
{
    /// <inheritdoc />
    public partial class ListAgendamentosEServicoAgendamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Servico",
                table: "Agendamentos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Servico",
                table: "Agendamentos");
        }
    }
}
