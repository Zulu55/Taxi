using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Taxi.Web.Migrations
{
    public partial class AddUserGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserGroups_UserGroupEntityId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserGroupEntityId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserGroupEntityId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "UserGroupDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    UserGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGroupDetails_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserGroupDetails_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserGroupRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProposalUserId = table.Column<string>(nullable: true),
                    RequiredUserId = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGroupRequests_AspNetUsers_ProposalUserId",
                        column: x => x.ProposalUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserGroupRequests_AspNetUsers_RequiredUserId",
                        column: x => x.RequiredUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupDetails_UserGroupId",
                table: "UserGroupDetails",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupDetails_UserId",
                table: "UserGroupDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupRequests_ProposalUserId",
                table: "UserGroupRequests",
                column: "ProposalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupRequests_RequiredUserId",
                table: "UserGroupRequests",
                column: "RequiredUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGroupDetails");

            migrationBuilder.DropTable(
                name: "UserGroupRequests");

            migrationBuilder.AddColumn<int>(
                name: "UserGroupEntityId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserGroupEntityId",
                table: "AspNetUsers",
                column: "UserGroupEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserGroups_UserGroupEntityId",
                table: "AspNetUsers",
                column: "UserGroupEntityId",
                principalTable: "UserGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
