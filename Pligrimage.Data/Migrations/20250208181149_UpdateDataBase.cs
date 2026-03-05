using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class UpdateDataBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_AlhajjMasters_PligrimageId",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_PligrimageId",
                table: "Passengers");

            migrationBuilder.AddColumn<int>(
                name: "AlhajjMasterPligrimageId",
                table: "Passengers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_AlhajjMasterPligrimageId",
                table: "Passengers",
                column: "AlhajjMasterPligrimageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_AlhajjMasters_AlhajjMasterPligrimageId",
                table: "Passengers",
                column: "AlhajjMasterPligrimageId",
                principalTable: "AlhajjMasters",
                principalColumn: "PligrimageId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_AlhajjMasters_AlhajjMasterPligrimageId",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_AlhajjMasterPligrimageId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "AlhajjMasterPligrimageId",
                table: "Passengers");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PligrimageId",
                table: "Passengers",
                column: "PligrimageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_AlhajjMasters_PligrimageId",
                table: "Passengers",
                column: "PligrimageId",
                principalTable: "AlhajjMasters",
                principalColumn: "PligrimageId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
