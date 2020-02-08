using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rankings.Infrastructure.Migrations
{
    public partial class AddDoubleGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoubleGames",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Player1Team1Id = table.Column<int>(nullable: true),
                    Player2Team1Id = table.Column<int>(nullable: true),
                    Player1Team2Id = table.Column<int>(nullable: true),
                    Player2Team2Id = table.Column<int>(nullable: true),
                    GameTypeId = table.Column<int>(nullable: true),
                    RegistrationDate = table.Column<DateTimeOffset>(nullable: false),
                    Score1 = table.Column<int>(nullable: false),
                    Score2 = table.Column<int>(nullable: false),
                    VenueId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoubleGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoubleGames_GameTypes_GameTypeId",
                        column: x => x.GameTypeId,
                        principalTable: "GameTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoubleGames_Profiles_Player1Team1Id",
                        column: x => x.Player1Team1Id,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoubleGames_Profiles_Player1Team2Id",
                        column: x => x.Player1Team2Id,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoubleGames_Profiles_Player2Team1Id",
                        column: x => x.Player2Team1Id,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoubleGames_Profiles_Player2Team2Id",
                        column: x => x.Player2Team2Id,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoubleGames_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoubleGames_GameTypeId",
                table: "DoubleGames",
                column: "GameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DoubleGames_Player1Team1Id",
                table: "DoubleGames",
                column: "Player1Team1Id");

            migrationBuilder.CreateIndex(
                name: "IX_DoubleGames_Player1Team2Id",
                table: "DoubleGames",
                column: "Player1Team2Id");

            migrationBuilder.CreateIndex(
                name: "IX_DoubleGames_Player2Team1Id",
                table: "DoubleGames",
                column: "Player2Team1Id");

            migrationBuilder.CreateIndex(
                name: "IX_DoubleGames_Player2Team2Id",
                table: "DoubleGames",
                column: "Player2Team2Id");

            migrationBuilder.CreateIndex(
                name: "IX_DoubleGames_VenueId",
                table: "DoubleGames",
                column: "VenueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoubleGames");
        }
    }
}
