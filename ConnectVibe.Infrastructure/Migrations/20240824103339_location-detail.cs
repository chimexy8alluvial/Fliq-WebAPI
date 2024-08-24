using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class locationdetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locationn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lat = table.Column<double>(type: "float", nullable: false),
                    Lng = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locationn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlusCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompoundCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GlobalCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlusCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Geometry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    LocationType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geometry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Geometry_Locationn_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locationn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlusCodeId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationDetails_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationDetails_PlusCode_PlusCodeId",
                        column: x => x.PlusCodeId,
                        principalTable: "PlusCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormattedAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeometryId = table.Column<int>(type: "int", nullable: false),
                    PlaceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlusCodeId = table.Column<int>(type: "int", nullable: false),
                    Types = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationDetailId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationResult_Geometry_GeometryId",
                        column: x => x.GeometryId,
                        principalTable: "Geometry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationResult_LocationDetails_LocationDetailId",
                        column: x => x.LocationDetailId,
                        principalTable: "LocationDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LocationResult_PlusCode_PlusCodeId",
                        column: x => x.PlusCodeId,
                        principalTable: "PlusCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressComponent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LongName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Types = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationResultId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressComponent_LocationResult_LocationResultId",
                        column: x => x.LocationResultId,
                        principalTable: "LocationResult",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressComponent_LocationResultId",
                table: "AddressComponent",
                column: "LocationResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Geometry_LocationId",
                table: "Geometry",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationDetails_LocationId",
                table: "LocationDetails",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationDetails_PlusCodeId",
                table: "LocationDetails",
                column: "PlusCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResult_GeometryId",
                table: "LocationResult",
                column: "GeometryId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResult_LocationDetailId",
                table: "LocationResult",
                column: "LocationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationResult_PlusCodeId",
                table: "LocationResult",
                column: "PlusCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressComponent");

            migrationBuilder.DropTable(
                name: "LocationResult");

            migrationBuilder.DropTable(
                name: "Geometry");

            migrationBuilder.DropTable(
                name: "LocationDetails");

            migrationBuilder.DropTable(
                name: "Locationn");

            migrationBuilder.DropTable(
                name: "PlusCode");
        }
    }
}
