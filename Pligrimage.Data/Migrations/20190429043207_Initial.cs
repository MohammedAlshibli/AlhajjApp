using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pligrimage.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DescArabic = table.Column<string>(nullable: true),
                    DescEnglish = table.Column<string>(nullable: true),
                    AlhajYear = table.Column<DateTime>(nullable: false),
                    QTY = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    DocumnetType = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentId);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    FlightId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FlightNo = table.Column<string>(nullable: true),
                    FlightDate = table.Column<DateTime>(nullable: false),
                    ArriveDate = table.Column<DateTime>(nullable: false),
                    FlightYear = table.Column<int>(nullable: false),
                    FlightCapacity = table.Column<int>(nullable: false),
                    Direction = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.FlightId);
                });

            migrationBuilder.CreateTable(
                name: "parameters",
                columns: table => new
                {
                    ParameterId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    DescArabic = table.Column<string>(nullable: true),
                    DescEnglish = table.Column<string>(nullable: true),
                    MaxValue = table.Column<int>(nullable: false),
                    MinValue = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parameters", x => x.ParameterId);
                });

            migrationBuilder.CreateTable(
                name: "Residences",
                columns: table => new
                {
                    ResidencesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Building = table.Column<string>(nullable: true),
                    Room = table.Column<int>(nullable: false),
                    RoomCapacity = table.Column<int>(nullable: false),
                    Floor = table.Column<int>(nullable: false),
                    Year = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residences", x => x.ResidencesId);
                });

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    UnitId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UnitNameEn = table.Column<string>(nullable: true),
                    UnitNameAr = table.Column<string>(nullable: true),
                    ModFlag = table.Column<bool>(nullable: false),
                    AlhajYear = table.Column<DateTime>(nullable: false),
                    AllowNumber = table.Column<int>(nullable: false),
                    StandBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units", x => x.UnitId);
                });

            migrationBuilder.CreateTable(
                name: "Buses",
                columns: table => new
                {
                    BusId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusNo = table.Column<string>(nullable: true),
                    BusCapacity = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    FlightId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buses", x => x.BusId);
                    table.ForeignKey(
                        name: "FK_Buses_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "FlightId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlhajjMasters",
                columns: table => new
                {
                    PligrimageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServcieNumber = table.Column<string>(nullable: false),
                    NIC = table.Column<int>(nullable: false),
                    NICExpire = table.Column<DateTime>(nullable: false),
                    Passport = table.Column<int>(nullable: false),
                    PassportExpire = table.Column<DateTime>(nullable: true),
                    FullName = table.Column<string>(maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    RankCode = table.Column<int>(nullable: false),
                    RankDesc = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: true),
                    Snapshote = table.Column<DateTime>(nullable: true),
                    UnitCode = table.Column<int>(nullable: false),
                    UnitDesc = table.Column<string>(nullable: true),
                    WorkLocation = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    WilayaCode = table.Column<int>(nullable: false),
                    WilayaDesc = table.Column<string>(nullable: true),
                    VillageCode = table.Column<int>(nullable: false),
                    VillageDesc = table.Column<string>(nullable: true),
                    WorkPhone = table.Column<string>(nullable: true),
                    GSM = table.Column<string>(nullable: true),
                    ReletiveName1 = table.Column<string>(nullable: true),
                    ReletiveCode1 = table.Column<int>(nullable: false),
                    RelativeGsm1 = table.Column<int>(nullable: false),
                    ReletiveName2 = table.Column<string>(nullable: true),
                    ReletiveCode2 = table.Column<int>(nullable: false),
                    RelativeGsm2 = table.Column<int>(nullable: false),
                    FitFlag = table.Column<bool>(nullable: false),
                    DoctorNote = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    AlhajYear = table.Column<int>(nullable: false),
                    BloodGroup = table.Column<string>(nullable: true),
                    DateOfEnlistment = table.Column<DateTime>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: true),
                    DocumentId = table.Column<int>(nullable: false),
                    ParameterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlhajjMasters", x => x.PligrimageId);
                    table.ForeignKey(
                        name: "FK_AlhajjMasters_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlhajjMasters_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlhajjMasters_parameters_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "parameters",
                        principalColumn: "ParameterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlhajjMasters_units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "units",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PassengerSupervisors",
                columns: table => new
                {
                    PassengerSuppId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    Year = table.Column<DateTime>(nullable: false),
                    PligrimageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassengerSupervisors", x => x.PassengerSuppId);
                    table.ForeignKey(
                        name: "FK_PassengerSupervisors_AlhajjMasters_PassengerSuppId",
                        column: x => x.PassengerSuppId,
                        principalTable: "AlhajjMasters",
                        principalColumn: "PligrimageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    PassengerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlhajYear = table.Column<int>(nullable: false),
                    PligrimageId = table.Column<int>(nullable: false),
                    FlightId = table.Column<int>(nullable: false),
                    BusId = table.Column<int>(nullable: false),
                    ResidencesId = table.Column<int>(nullable: false),
                    PassengerSuppId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.PassengerId);
                    table.ForeignKey(
                        name: "FK_Passengers_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "BusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Passengers_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "FlightId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Passengers_PassengerSupervisors_PassengerSuppId",
                        column: x => x.PassengerSuppId,
                        principalTable: "PassengerSupervisors",
                        principalColumn: "PassengerSuppId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Passengers_AlhajjMasters_PligrimageId",
                        column: x => x.PligrimageId,
                        principalTable: "AlhajjMasters",
                        principalColumn: "PligrimageId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Passengers_Residences_ResidencesId",
                        column: x => x.ResidencesId,
                        principalTable: "Residences",
                        principalColumn: "ResidencesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlhajjMasters_CategoryId",
                table: "AlhajjMasters",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AlhajjMasters_DocumentId",
                table: "AlhajjMasters",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AlhajjMasters_NIC",
                table: "AlhajjMasters",
                column: "NIC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlhajjMasters_ParameterId",
                table: "AlhajjMasters",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_AlhajjMasters_UnitId",
                table: "AlhajjMasters",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Buses_FlightId",
                table: "Buses",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_BusId",
                table: "Passengers",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_FlightId",
                table: "Passengers",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PassengerSuppId",
                table: "Passengers",
                column: "PassengerSuppId");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PligrimageId",
                table: "Passengers",
                column: "PligrimageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_ResidencesId",
                table: "Passengers",
                column: "ResidencesId");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PligrimageId_AlhajYear",
                table: "Passengers",
                columns: new[] { "PligrimageId", "AlhajYear" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Passengers");

            migrationBuilder.DropTable(
                name: "Buses");

            migrationBuilder.DropTable(
                name: "PassengerSupervisors");

            migrationBuilder.DropTable(
                name: "Residences");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "AlhajjMasters");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "parameters");

            migrationBuilder.DropTable(
                name: "units");
        }
    }
}
