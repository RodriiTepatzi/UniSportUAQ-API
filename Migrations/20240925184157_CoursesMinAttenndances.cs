using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    /// <inheritdoc />
    public partial class CoursesMinAttenndances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinAttendances",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinAttendances",
                table: "Courses");
        }
    }
}
