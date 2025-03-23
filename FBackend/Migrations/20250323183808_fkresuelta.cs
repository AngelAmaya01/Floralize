using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBackend.Migrations
{
    /// <inheritdoc />
    public partial class fkresuelta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Proveedores_ProveedoresId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ProveedoresId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ProveedoresId",
                table: "Productos");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos",
                column: "ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos");

            migrationBuilder.AddColumn<Guid>(
                name: "ProveedoresId",
                table: "Productos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedoresId",
                table: "Productos",
                column: "ProveedoresId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Proveedores_ProveedoresId",
                table: "Productos",
                column: "ProveedoresId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
