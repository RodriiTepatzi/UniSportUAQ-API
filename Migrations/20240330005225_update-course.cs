using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    /// <inheritdoc />
    public partial class updatecourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hour",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "EndHour",
                table: "Courses",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartHour",
                table: "Courses",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndHour",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StartHour",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "Hour",
                table: "Courses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
