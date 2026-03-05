using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class photpath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "AlhajjMasters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "AlhajjMasters");
        }
    }
}
