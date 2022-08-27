using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackerPetProj.Migrations
{
    public partial class AddTimePropertiesToTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeEstimate",
                table: "Tickets",
                newName: "CreationTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Tickets",
                newName: "TimeEstimate");
        }
    }
}
