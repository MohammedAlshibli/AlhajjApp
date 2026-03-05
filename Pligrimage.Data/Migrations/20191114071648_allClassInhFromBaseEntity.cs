using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class allClassInhFromBaseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "units",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateOn",
                table: "units",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "units",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "units",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "Residences",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateOn",
                table: "Residences",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Residences",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Residences",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "PassengerSupervisors",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateOn",
                table: "PassengerSupervisors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "PassengerSupervisors",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "PassengerSupervisors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "Passengers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateOn",
                table: "Passengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Passengers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Passengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "parameters",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateOn",
                table: "parameters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "parameters",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "parameters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "Flights",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateOn",
                table: "Flights",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Flights",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Flights",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateOn",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "categories",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateOn",
                table: "categories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "categories",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "categories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "units");

            migrationBuilder.DropColumn(
                name: "CreateOn",
                table: "units");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "units");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "units");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "CreateOn",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "PassengerSupervisors");

            migrationBuilder.DropColumn(
                name: "CreateOn",
                table: "PassengerSupervisors");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PassengerSupervisors");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "PassengerSupervisors");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "CreateOn",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "parameters");

            migrationBuilder.DropColumn(
                name: "CreateOn",
                table: "parameters");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "parameters");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "parameters");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "CreateOn",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CreateOn",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "CreateOn",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "categories");
        }
    }
}
