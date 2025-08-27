using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VacationPlanner.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ExternalId",
                table: "Employees",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_ExternalId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Employees");
        }
    }
}
