using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class forNullData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlhajjMasters_categories_CategoryId",
                table: "AlhajjMasters");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "AlhajjMasters",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_AlhajjMasters_categories_CategoryId",
                table: "AlhajjMasters",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlhajjMasters_categories_CategoryId",
                table: "AlhajjMasters");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AlhajjMasters_categories_CategoryId",
                table: "AlhajjMasters",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
