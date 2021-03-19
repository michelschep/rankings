using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rankings.Infrastructure.Migrations
{
    public partial class AddGamesProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameProjections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(type: "TEXT", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GameType = table.Column<string>(type: "TEXT", nullable: true),
                    Venue = table.Column<string>(type: "TEXT", nullable: true),
                    FirstPlayerId = table.Column<string>(type: "TEXT", nullable: true),
                    FirstPlayerName = table.Column<string>(type: "TEXT", nullable: true),
                    SecondPlayerId = table.Column<string>(type: "TEXT", nullable: true),
                    SecondPlayerName = table.Column<string>(type: "TEXT", nullable: true),
                    Score1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Score2 = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    EloFirstPlayer = table.Column<string>(type: "TEXT", nullable: true),
                    EloSecondPlayer = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProjections", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameProjections");
        }
    }
}
