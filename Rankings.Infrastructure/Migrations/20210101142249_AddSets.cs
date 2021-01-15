using Microsoft.EntityFrameworkCore.Migrations;

namespace Rankings.Infrastructure.Migrations
{
    public partial class AddSets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SetScores1",
                table: "Games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SetScores2",
                table: "Games",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetScores1",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SetScores2",
                table: "Games");
        }
    }
}
