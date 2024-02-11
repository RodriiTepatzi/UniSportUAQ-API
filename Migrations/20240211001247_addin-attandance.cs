using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    /// <inheritdoc />
    public partial class addinattandance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdStudent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdCourse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttendanceClass = table.Column<bool>(type: "bit", nullable: false),
                    Id_Student = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Id_Course = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Courses_Id_Course",
                        column: x => x.Id_Course,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attendances_Students_Id_Student",
                        column: x => x.Id_Student,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_Id_Course",
                table: "Attendances",
                column: "Id_Course");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_Id_Student",
                table: "Attendances",
                column: "Id_Student");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendances");
        }
    }
}
