using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class HrmsUnitCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitDesc",
                table: "AlhajjMasters",
                newName: "HrmsUnitDesc");

            migrationBuilder.RenameColumn(
                name: "UnitCode",
                table: "AlhajjMasters",
                newName: "HrmsUnitCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HrmsUnitDesc",
                table: "AlhajjMasters",
                newName: "UnitDesc");

            migrationBuilder.RenameColumn(
                name: "HrmsUnitCode",
                table: "AlhajjMasters",
                newName: "UnitCode");
        }
    }
}
