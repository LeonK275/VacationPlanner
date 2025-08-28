using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VacationPlanner.Migrations
{
    /// <inheritdoc />
    public partial class RecreateInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_ExternalId",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "Employees",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ExternalId",
                table: "Employees",
                column: "ExternalId",
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_ExternalId",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ExternalId",
                table: "Employees",
                column: "ExternalId",
                unique: true);
        }
    }
}
