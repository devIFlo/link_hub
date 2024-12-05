using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSerilog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Logs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Logs",
                type: "TEXT",
                nullable: true);
        }
    }
}
