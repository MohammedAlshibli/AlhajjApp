using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class alshekDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SheikhGsm",
                table: "AlhajjMasters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SheikhName",
                table: "AlhajjMasters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SheikhGsm",
                table: "AlhajjMasters");

            migrationBuilder.DropColumn(
                name: "SheikhName",
                table: "AlhajjMasters");
        }
    }
}
