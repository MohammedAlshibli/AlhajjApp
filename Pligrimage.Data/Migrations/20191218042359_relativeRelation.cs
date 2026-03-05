using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class relativeRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReletiveCode1",
                table: "AlhajjMasters");

            migrationBuilder.DropColumn(
                name: "ReletiveCode2",
                table: "AlhajjMasters");

            migrationBuilder.AddColumn<string>(
                name: "ReletiveRelation1",
                table: "AlhajjMasters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReletiveRelation2",
                table: "AlhajjMasters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReletiveRelation1",
                table: "AlhajjMasters");

            migrationBuilder.DropColumn(
                name: "ReletiveRelation2",
                table: "AlhajjMasters");

            migrationBuilder.AddColumn<int>(
                name: "ReletiveCode1",
                table: "AlhajjMasters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReletiveCode2",
                table: "AlhajjMasters",
                nullable: false,
                defaultValue: 0);
        }
    }
}
