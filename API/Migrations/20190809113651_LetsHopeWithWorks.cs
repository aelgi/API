using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class LetsHopeWithWorks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Projects_ProjectId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "BaseUserId",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Items",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BaseUserId",
                table: "Projects",
                column: "BaseUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Projects_ProjectId",
                table: "Items",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_BaseUserId",
                table: "Projects",
                column: "BaseUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Projects_ProjectId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_BaseUserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_BaseUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "BaseUserId",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Projects",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Items",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Projects_ProjectId",
                table: "Items",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_UserId",
                table: "Projects",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
