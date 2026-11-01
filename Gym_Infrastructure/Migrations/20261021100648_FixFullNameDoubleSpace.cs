using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixFullNameDoubleSpace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "CONCAT_WS(' ', [FirstName], NULLIF([MiddleName], ''), [LastName])",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComputedColumnSql: "[FirstName] + ' ' + ISNULL([MiddleName], '') + ' ' + [LastName]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "[FirstName] + ' ' + ISNULL([MiddleName], '') + ' ' + [LastName]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComputedColumnSql: "CONCAT_WS(' ', [FirstName], NULLIF([MiddleName], ''), [LastName])");
        }
    }
}
