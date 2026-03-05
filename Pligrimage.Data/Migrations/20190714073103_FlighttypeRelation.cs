using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class FlighttypeRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParameterId",
                table: "Flights",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ParameterId",
                table: "Flights",
                column: "ParameterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_parameters_ParameterId",
                table: "Flights",
                column: "ParameterId",
                principalTable: "parameters",
                principalColumn: "ParameterId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_parameters_ParameterId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_ParameterId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "ParameterId",
                table: "Flights");
        }
    }
}
