using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
	/// <inheritdoc />
#pragma warning disable CS8981 // El nombre de tipo solo contiene caracteres ASCII en minúsculas. Estos nombres pueden reservarse para el idioma.
	public partial class fixingusers : Migration
#pragma warning restore CS8981 // El nombre de tipo solo contiene caracteres ASCII en minúsculas. Estos nombres pueden reservarse para el idioma.
	{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_ApplicationUsers_StudentId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_CartasLiberacion_ApplicationUsers_StudentId",
                table: "CartasLiberacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Inscriptions_ApplicationUsers_StudentId",
                table: "Inscriptions");


            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_ApplicationUsers_StudentId",
                table: "Attendances",
                column: "StudentId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartasLiberacion_ApplicationUsers_StudentId",
                table: "CartasLiberacion",
                column: "StudentId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inscriptions_ApplicationUsers_StudentId",
                table: "Inscriptions",
                column: "StudentId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_ApplicationUsers_StudentId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_CartasLiberacion_ApplicationUsers_StudentId",
                table: "CartasLiberacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Inscriptions_ApplicationUsers_StudentId",
                table: "Inscriptions");

            migrationBuilder.AddColumn<string>(
                name: "CurrentCourseId",
                table: "ApplicationUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_CurrentCourseId",
                table: "ApplicationUsers",
                column: "CurrentCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_Inscriptions_CurrentCourseId",
                table: "ApplicationUsers",
                column: "CurrentCourseId",
                principalTable: "Inscriptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_ApplicationUsers_StudentId",
                table: "Attendances",
                column: "StudentId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartasLiberacion_ApplicationUsers_StudentId",
                table: "CartasLiberacion",
                column: "StudentId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inscriptions_ApplicationUsers_StudentId",
                table: "Inscriptions",
                column: "StudentId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
