using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class changFlightType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Passengers_PligrimageId",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_PligrimageId_AlhajYear",
                table: "Passengers");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PligrimageId",
                table: "Passengers",
                column: "PligrimageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Passengers_PligrimageId",
                table: "Passengers");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PligrimageId",
                table: "Passengers",
                column: "PligrimageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PligrimageId_AlhajYear",
                table: "Passengers",
                columns: new[] { "PligrimageId", "AlhajYear" },
                unique: true);
        }
    }
}
