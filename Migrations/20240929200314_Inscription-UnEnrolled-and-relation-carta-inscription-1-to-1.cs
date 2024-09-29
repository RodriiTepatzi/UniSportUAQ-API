using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    /// <inheritdoc />
    public partial class InscriptionUnEnrolledandrelationcartainscription1to1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CartaId",
                table: "Inscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UnEnrolled",
                table: "Inscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InscriptionId",
                table: "CartasLiberacion",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartasLiberacion_InscriptionId",
                table: "CartasLiberacion",
                column: "InscriptionId",
                unique: true,
                filter: "[InscriptionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CartasLiberacion_Inscriptions_InscriptionId",
                table: "CartasLiberacion",
                column: "InscriptionId",
                principalTable: "Inscriptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartasLiberacion_Inscriptions_InscriptionId",
                table: "CartasLiberacion");

            migrationBuilder.DropIndex(
                name: "IX_CartasLiberacion_InscriptionId",
                table: "CartasLiberacion");

            migrationBuilder.DropColumn(
                name: "CartaId",
                table: "Inscriptions");

            migrationBuilder.DropColumn(
                name: "UnEnrolled",
                table: "Inscriptions");

            migrationBuilder.DropColumn(
                name: "InscriptionId",
                table: "CartasLiberacion");
        }
    }
}
