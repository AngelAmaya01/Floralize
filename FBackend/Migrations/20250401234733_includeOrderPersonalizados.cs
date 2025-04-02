using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBackend.Migrations
{
    /// <inheritdoc />
    public partial class includeOrderPersonalizados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Personalizados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoFlor = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<string>(type: "text", nullable: false),
                    IncluirPresente = table.Column<string>(type: "text", nullable: false),
                    IncluirBase = table.Column<string>(type: "text", nullable: false),
                    FotoReferenciaURL = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personalizados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personalizados_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personalizados_UserId",
                table: "Personalizados",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Personalizados");
        }
    }
}
