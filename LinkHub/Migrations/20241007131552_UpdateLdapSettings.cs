using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLdapSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Host",
                table: "LdapSettings",
                newName: "NetBiosDomain");

            migrationBuilder.RenameColumn(
                name: "Domain",
                table: "LdapSettings",
                newName: "FqdnDomain");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NetBiosDomain",
                table: "LdapSettings",
                newName: "Host");

            migrationBuilder.RenameColumn(
                name: "FqdnDomain",
                table: "LdapSettings",
                newName: "Domain");
        }
    }
}
