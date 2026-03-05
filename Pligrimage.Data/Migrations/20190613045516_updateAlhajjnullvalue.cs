using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class updateAlhajjnullvalue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AlhajjMasters_NIC",
                table: "AlhajjMasters");

            migrationBuilder.AlterColumn<int>(
                name: "ReletiveCode2",
                table: "AlhajjMasters",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ReletiveCode1",
                table: "AlhajjMasters",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "RelativeGsm2",
                table: "AlhajjMasters",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "RelativeGsm1",
                table: "AlhajjMasters",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "RankCode",
                table: "AlhajjMasters",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "Passport",
                table: "AlhajjMasters",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "NICExpire",
                table: "AlhajjMasters",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<int>(
                name: "NIC",
                table: "AlhajjMasters",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_AlhajjMasters_NIC",
                table: "AlhajjMasters",
                column: "NIC",
                unique: true,
                filter: "[NIC] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AlhajjMasters_NIC",
                table: "AlhajjMasters");

            migrationBuilder.AlterColumn<int>(
                name: "ReletiveCode2",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReletiveCode1",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RelativeGsm2",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RelativeGsm1",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RankCode",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Passport",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NICExpire",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NIC",
                table: "AlhajjMasters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlhajjMasters_NIC",
                table: "AlhajjMasters",
                column: "NIC",
                unique: true);
        }
    }
}
