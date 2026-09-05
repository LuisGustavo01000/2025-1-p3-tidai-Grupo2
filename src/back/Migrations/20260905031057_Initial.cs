using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourProject.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CONTEUDO",
                columns: table => new
                {
                    ID_CONTEUDO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TITULO_CONTEUDO = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DESCRICAO_CONTEUDO = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TIPO_CONTEUDO = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NIVEL_CONTEUDO = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DATA_PUBLICACAO = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    USUARIOFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTEUDO", x => x.ID_CONTEUDO);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NOME_USUARIO = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EMAIL_USUARIO = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SENHA_USUARIO = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ENDIVIDADO = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DATA_CRIACAO = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.ID_USUARIO);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DASHBOARD",
                columns: table => new
                {
                    ID_DASH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USUARIOFK = table.Column<int>(type: "int", nullable: false),
                    SALDOTOTAL_DASH = table.Column<double>(type: "double", nullable: false),
                    INVESTIMENTOTOTAIS_DASH = table.Column<double>(type: "double", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "META_FINANCEIRA",
                columns: table => new
                {
                    ID_META = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NOME_META = table.Column<string>(type: "varchar(56)", maxLength: 56, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VALOR_META = table.Column<double>(type: "double", nullable: false),
                    PRAZO_META = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    STATUS_META = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    USUARIOFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_META_FINANCEIRA", x => x.ID_META);
                    table.ForeignKey(
                        name: "FK_META_FINANCEIRA_USUARIO_USUARIOFK",
                        column: x => x.USUARIOFK,
                        principalTable: "USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TRANSACAO",
                columns: table => new
                {
                    ID_TRANSACAO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DESCRICAO_CONT = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VALOR_TRANS = table.Column<double>(type: "double", nullable: false),
                    TIPO_TRANS = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DATA_TRANS = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    USUARIOFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRANSACAO", x => x.ID_TRANSACAO);
                    table.ForeignKey(
                        name: "FK_TRANSACAO_USUARIO_USUARIOFK",
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

            migrationBuilder.CreateIndex(
                name: "IX_META_FINANCEIRA_USUARIOFK",
                table: "META_FINANCEIRA",
                column: "USUARIOFK");

            migrationBuilder.CreateIndex(
                name: "IX_TRANSACAO_USUARIOFK",
                table: "TRANSACAO",
                column: "USUARIOFK");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_EMAIL_USUARIO",
                table: "USUARIO",
                column: "EMAIL_USUARIO",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONTEUDO");

            migrationBuilder.DropTable(
                name: "DASHBOARD");

            migrationBuilder.DropTable(
                name: "META_FINANCEIRA");

            migrationBuilder.DropTable(
                name: "TRANSACAO");

            migrationBuilder.DropTable(
                name: "USUARIO");
        }
    }
}
