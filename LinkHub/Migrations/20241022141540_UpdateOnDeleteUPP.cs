using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOnDeleteUPP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPagePermissions_AspNetUsers_UserId",
                table: "UserPagePermissions");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPagePermissions_AspNetUsers_UserId",
                table: "UserPagePermissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPagePermissions_AspNetUsers_UserId",
                table: "UserPagePermissions");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPagePermissions_AspNetUsers_UserId",
                table: "UserPagePermissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
