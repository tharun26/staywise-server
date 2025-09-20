using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Staywise.Migrations
{
    /// <inheritdoc />
    public partial class AddListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Listings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PricePerNight = table.Column<decimal>(type: "numeric", nullable: false),
                    Address_Line1 = table.Column<string>(type: "text", nullable: false),
                    Address_Line2 = table.Column<string>(type: "text", nullable: false),
                    Address_City = table.Column<string>(type: "text", nullable: false),
                    Address_State = table.Column<string>(type: "text", nullable: false),
                    Address_Country = table.Column<string>(type: "text", nullable: false),
                    Address_PostalCode = table.Column<int>(type: "integer", nullable: false),
                    Address_Location_Type = table.Column<string>(type: "text", nullable: false),
                    Address_Location_Coordinates = table.Column<double[]>(type: "double precision[]", nullable: false),
                    Amenities = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Photos = table.Column<List<string>>(type: "jsonb", nullable: false),
                    MaxGuests = table.Column<int>(type: "integer", nullable: false),
                    BedRooms = table.Column<int>(type: "integer", nullable: false),
                    BathRooms = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listings_Users_HostId",
                        column: x => x.HostId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Listings_HostId",
                table: "Listings",
                column: "HostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Listings");
        }
    }
}
