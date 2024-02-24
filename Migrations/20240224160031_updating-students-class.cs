using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    /// <inheritdoc />
    public partial class updatingstudentsclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_CourseClasses_CurrentCourse",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_CurrentCourse",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CurrentCourse",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "SuscribedDateTime",
                table: "Students",
                newName: "RegisteredDateTime");

            migrationBuilder.RenameColumn(
                name: "IsSuscribed",
                table: "Students",
                newName: "IsInFIF");

            migrationBuilder.RenameColumn(
                name: "IsInOfficialGroup",
                table: "Students",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "FinishedCoursesJson",
                table: "Students",
                newName: "CurrentCourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisteredDateTime",
                table: "Students",
                newName: "SuscribedDateTime");

            migrationBuilder.RenameColumn(
                name: "IsInFIF",
                table: "Students",
                newName: "IsSuscribed");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Students",
                newName: "IsInOfficialGroup");

            migrationBuilder.RenameColumn(
                name: "CurrentCourseId",
                table: "Students",
                newName: "FinishedCoursesJson");

            migrationBuilder.AddColumn<string>(
                name: "CurrentCourse",
                table: "Students",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_CurrentCourse",
                table: "Students",
                column: "CurrentCourse");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_CourseClasses_CurrentCourse",
                table: "Students",
                column: "CurrentCourse",
                principalTable: "CourseClasses",
                principalColumn: "Id");
        }
    }
}
