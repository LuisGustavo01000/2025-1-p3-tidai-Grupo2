using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourProject.Migrations
{
    /// <inheritdoc />
    public partial class AddConteudoUsuarioFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DATA_PUBLICACAO",
                table: "CONTEUDO",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.CreateIndex(
                name: "IX_CONTEUDO_USUARIOFK",
                table: "CONTEUDO",
                column: "USUARIOFK");

            migrationBuilder.AddForeignKey(
                name: "FK_CONTEUDO_USUARIO_USUARIOFK",
                table: "CONTEUDO",
                column: "USUARIOFK",
                principalTable: "USUARIO",
                principalColumn: "ID_USUARIO",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CONTEUDO_USUARIO_USUARIOFK",
                table: "CONTEUDO");

            migrationBuilder.DropIndex(
                name: "IX_CONTEUDO_USUARIOFK",
                table: "CONTEUDO");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DATA_PUBLICACAO",
                table: "CONTEUDO",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "CURRENT_TIMESTAMP(6)");
        }
    }
}
