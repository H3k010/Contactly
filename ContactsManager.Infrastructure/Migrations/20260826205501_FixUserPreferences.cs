using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE `AspNetUsers` SET `UserPreferences_PageSize` = 25 WHERE `UserPreferences_PageSize` = 0;");
            migrationBuilder.Sql("UPDATE `AspNetUsers` SET `UserPreferences_SortBy` = 'Name' WHERE `UserPreferences_SortBy` = '';");
            migrationBuilder.Sql("UPDATE `AspNetUsers` SET `UserPreferences_SortOrder` = 'ASC' WHERE `UserPreferences_SortOrder` = '';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE `AspNetUsers` SET `UserPreferences_PageSize` = 0 WHERE `UserPreferences_PageSize` = 25;");
            migrationBuilder.Sql("UPDATE `AspNetUsers` SET `UserPreferences_SortBy` = '' WHERE `UserPreferences_SortBy` = 'Name';");
            migrationBuilder.Sql("UPDATE `AspNetUsers` SET `UserPreferences_SortOrder` = '' WHERE `UserPreferences_SortOrder` = 'ASC';");
        }
    }
    
}
