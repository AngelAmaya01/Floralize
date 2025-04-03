using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBackend.Migrations
{
    /// <inheritdoc />
    public partial class servicePerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoBase",
                table: "Personalizados",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoPresente",
                table: "Personalizados",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoBase",
                table: "Personalizados");

            migrationBuilder.DropColumn(
                name: "TipoPresente",
                table: "Personalizados");
        }
    }
}
