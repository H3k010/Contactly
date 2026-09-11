using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserPreferences_PageSize",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPreferences_SortBy",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPreferences_SortOrder",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true);
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
