using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    /// <inheritdoc />
    public partial class updatebd3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inscriptions_CourseClasses_CourseId",
                table: "Inscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscriptions_Courses_CourseId",
                table: "Inscriptions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inscriptions_Courses_CourseId",
                table: "Inscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscriptions_CourseClasses_CourseId",
                table: "Inscriptions",
                column: "CourseId",
                principalTable: "CourseClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
