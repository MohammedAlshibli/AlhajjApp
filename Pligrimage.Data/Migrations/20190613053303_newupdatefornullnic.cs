using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class newupdatefornullnic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
    name: "IX_AlhajjMasters_NIC",
    table: "AlhajjMasters");
            migrationBuilder.AlterColumn<string>(
                name: "NIC",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
              name: "IX_AlhajjMasters_NIC",
              table: "AlhajjMasters");
            migrationBuilder.AlterColumn<int>(
                name: "NIC",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
