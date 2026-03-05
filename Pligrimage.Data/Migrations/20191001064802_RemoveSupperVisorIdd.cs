using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class RemoveSupperVisorIdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_PassengerSupervisors_PassengerSuppId",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_PassengerSuppId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "PassengerSuppId",
                table: "Passengers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PassengerSuppId",
                table: "Passengers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PassengerSuppId",
                table: "Passengers",
                column: "PassengerSuppId");

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_PassengerSupervisors_PassengerSuppId",
                table: "Passengers",
                column: "PassengerSuppId",
                principalTable: "PassengerSupervisors",
                principalColumn: "PassengerSuppId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
