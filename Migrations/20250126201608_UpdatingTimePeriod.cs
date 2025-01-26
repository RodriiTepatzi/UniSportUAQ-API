using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingTimePeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Period",
                table: "TimePeriods");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TimePeriods");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TimePeriods",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TimePeriods");

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "TimePeriods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TimePeriods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
