using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourProject.Migrations
{
    /// <inheritdoc />
    public partial class AddTransacaoCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CATEGORIA_TRANS",
                table: "TRANSACAO",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Outros")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CATEGORIA_TRANS",
                table: "TRANSACAO");
        }
    }
}
