using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Staywise.Migrations
{
    /// <inheritdoc />
    public partial class SplittingAddressModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Address_Line1",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Address_Line2",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Address_Location_Coordinates",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Address_Location_Type",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Address_State",
                table: "Listings");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Listings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Line1 = table.Column<string>(type: "text", nullable: false),
                    Line2 = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    PostalCode = table.Column<int>(type: "integer", nullable: false),
                    Location_Type = table.Column<string>(type: "text", nullable: false),
                    Location_Coordinates = table.Column<double[]>(type: "double precision[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Listings_AddressId",
                table: "Listings",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Addresses_AddressId",
                table: "Listings",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Addresses_AddressId",
                table: "Listings");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Listings_AddressId",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Listings");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Listings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                table: "Listings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Line1",
                table: "Listings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Line2",
                table: "Listings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double[]>(
                name: "Address_Location_Coordinates",
                table: "Listings",
                type: "double precision[]",
                nullable: false,
                defaultValue: new double[0]);

            migrationBuilder.AddColumn<string>(
                name: "Address_Location_Type",
                table: "Listings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Address_PostalCode",
                table: "Listings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Address_State",
                table: "Listings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
