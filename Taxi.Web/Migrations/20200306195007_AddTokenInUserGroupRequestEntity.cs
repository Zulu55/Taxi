using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Taxi.Web.Migrations
{
    public partial class AddTokenInUserGroupRequestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Token",
                table: "UserGroupRequests",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "UserGroupRequests");
        }
    }
}
