using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    /// <inheritdoc />
    public partial class addingstudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
