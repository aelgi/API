using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class StillCantWorkTakeTwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Projects_ProjectId1",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_BaseUserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_BaseUserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Items_ProjectId1",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BaseUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                table: "Items");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseUserId",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId1",
                table: "Items",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BaseUserId",
                table: "Projects",
                column: "BaseUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ProjectId1",
                table: "Items",
                column: "ProjectId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Projects_ProjectId1",
                table: "Items",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_BaseUserId",
                table: "Projects",
                column: "BaseUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
