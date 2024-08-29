using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableUserPagePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPagePermissions_AspNetUsers_UserId",
                table: "UserPagePermissions");

            migrationBuilder.DropColumn(
                name: "CanEdit",
                table: "UserPagePermissions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserPagePermissions",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPagePermissions_AspNetUsers_UserId",
                table: "UserPagePermissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPagePermissions_AspNetUsers_UserId",
                table: "UserPagePermissions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserPagePermissions",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanEdit",
                table: "UserPagePermissions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPagePermissions_AspNetUsers_UserId",
                table: "UserPagePermissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
