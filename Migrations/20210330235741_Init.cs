using Microsoft.EntityFrameworkCore.Migrations;

namespace TGBotCSharp.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Langs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LangCode = table.Column<string>(type: "TEXT", nullable: false),
                    FriendlyTitle = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Langs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    SrcLangId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToLangId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Langs_SrcLangId",
                        column: x => x.SrcLangId,
                        principalTable: "Langs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Langs_ToLangId",
                        column: x => x.ToLangId,
                        principalTable: "Langs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_SrcLangId",
                table: "Users",
                column: "SrcLangId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ToLangId",
                table: "Users",
                column: "ToLangId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Langs");
        }
    }
}
