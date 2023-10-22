using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contacts.WebAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastName",
                value: "Nowak [BD]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastName",
                value: "Nowak");
        }
    }
}
