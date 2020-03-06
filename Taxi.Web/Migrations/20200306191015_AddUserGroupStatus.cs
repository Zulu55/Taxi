using Microsoft.EntityFrameworkCore.Migrations;

namespace Taxi.Web.Migrations
{
    public partial class AddUserGroupStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasAccepted",
                table: "UserGroupRequests");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserGroupRequests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserGroupRequests");

            migrationBuilder.AddColumn<bool>(
                name: "WasAccepted",
                table: "UserGroupRequests",
                nullable: false,
                defaultValue: false);
        }
    }
}
