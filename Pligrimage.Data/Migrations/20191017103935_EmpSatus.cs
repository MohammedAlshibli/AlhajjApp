using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class EmpSatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeStatus",
                table: "AlhajjMasters",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeStatus",
                table: "AlhajjMasters");
        }
    }
}
