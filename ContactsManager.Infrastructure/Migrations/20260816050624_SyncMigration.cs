using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Contacts",
                type: "VARCHAR(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Contacts",
                type: "VARCHAR(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Contacts",
                type: "VARCHAR(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contacts",
                type: "VARCHAR(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Contacts",
                type: "VARCHAR(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Contacts",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Contacts");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Contacts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(255)");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Contacts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Contacts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contacts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Contacts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(255)");
        }
    }
}
