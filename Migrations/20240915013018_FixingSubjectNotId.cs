using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    /// <inheritdoc />
    public partial class FixingSubjectNotId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subject_ApplicationUsers_InstructorId",
                table: "Subject");

            migrationBuilder.DropIndex(
                name: "IX_Subject_InstructorId",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Subject");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstructorId",
                table: "Subject",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_InstructorId",
                table: "Subject",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_ApplicationUsers_InstructorId",
                table: "Subject",
                column: "InstructorId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
