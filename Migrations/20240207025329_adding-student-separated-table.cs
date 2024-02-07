using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    /// <inheritdoc />
    public partial class addingstudentseparatedtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentCourse",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "FinishedCoursesJson",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "IsInOfficialGroup",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "IsSuscribed",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "StudyPlan",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "SuscribedDateTime",
                table: "ApplicationUsers");

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Group = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    IsInOfficialGroup = table.Column<bool>(type: "bit", nullable: false),
                    IsSuscribed = table.Column<bool>(type: "bit", nullable: false),
                    StudyPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinishedCoursesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuscribedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentCourse = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_ApplicationUsers_Id",
                        column: x => x.Id,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.AddColumn<int>(
                name: "CurrentCourse",
                table: "ApplicationUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FinishedCoursesJson",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "ApplicationUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInOfficialGroup",
                table: "ApplicationUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuscribed",
                table: "ApplicationUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "ApplicationUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudyPlan",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuscribedDateTime",
                table: "ApplicationUsers",
                type: "datetime2",
                nullable: true);
        }
    }
}
