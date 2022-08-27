using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackerPetProj.Migrations
{
    public partial class AddAuthorToTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Tickets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Tickets");
        }
    }
}
