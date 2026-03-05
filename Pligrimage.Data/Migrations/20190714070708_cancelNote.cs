using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class cancelNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelNote",
                table: "AlhajjMasters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelNote",
                table: "AlhajjMasters");
        }
    }
}
