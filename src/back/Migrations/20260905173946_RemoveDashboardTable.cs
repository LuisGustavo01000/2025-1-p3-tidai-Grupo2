using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourProject.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDashboardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DASHBOARD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DASHBOARD",
                columns: table => new
                {
                    ID_DASH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USUARIOFK = table.Column<int>(type: "int", nullable: false),
                    INVESTIMENTOTOTAIS_DASH = table.Column<double>(type: "double", nullable: false),
                    SALDOTOTAL_DASH = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DASHBOARD", x => x.ID_DASH);
                    table.ForeignKey(
                        name: "FK_DASHBOARD_USUARIO_USUARIOFK",
                        column: x => x.USUARIOFK,
                        principalTable: "USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DASHBOARD_USUARIOFK",
                table: "DASHBOARD",
                column: "USUARIOFK");
        }
    }
}
