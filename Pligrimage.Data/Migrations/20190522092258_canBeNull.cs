using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class canBeNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlhajjMasters_Documents_DocumentId",
                table: "AlhajjMasters");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "AlhajjMasters",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_AlhajjMasters_Documents_DocumentId",
                table: "AlhajjMasters",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "DocumentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlhajjMasters_Documents_DocumentId",
                table: "AlhajjMasters");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AlhajjMasters_Documents_DocumentId",
                table: "AlhajjMasters",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "DocumentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
