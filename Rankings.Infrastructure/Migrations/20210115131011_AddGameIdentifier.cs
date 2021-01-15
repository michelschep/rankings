using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rankings.Infrastructure.Migrations
{
    public partial class AddGameIdentifier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Identifier",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "Games");
        }
    }
}
