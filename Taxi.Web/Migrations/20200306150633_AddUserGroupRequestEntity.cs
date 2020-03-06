using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Taxi.Web.Migrations
{
    public partial class AddUserGroupRequestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGroupRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProposalUserId = table.Column<string>(nullable: true),
                    RequiredUserId = table.Column<string>(nullable: true),
                    WasAccepted = table.Column<bool>(nullable: false)
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
                name: "UserGroupRequests");
        }
    }
}
