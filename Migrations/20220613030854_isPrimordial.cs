using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JATS.Migrations
{
    public partial class isPrimordial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPrimordial",
                table: "Tickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isPrimordial",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPrimordial",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "isPrimordial",
                table: "Projects");
        }
    }
}
