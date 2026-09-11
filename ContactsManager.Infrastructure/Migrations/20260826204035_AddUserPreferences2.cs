using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreferences2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserPreferences_PageSize",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserPreferences_SortBy",
                table: "AspNetUsers",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserPreferences_SortOrder",
                table: "AspNetUsers",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPreferences_PageSize",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserPreferences_SortBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserPreferences_SortOrder",
                table: "AspNetUsers");
        }
    }
}
