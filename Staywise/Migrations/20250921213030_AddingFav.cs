using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Staywise.Migrations
{
    /// <inheritdoc />
    public partial class AddingFav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    FavoriteListingsId = table.Column<Guid>(type: "uuid", nullable: false),
                    FavoritedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => new { x.FavoriteListingsId, x.FavoritedById });
                    table.ForeignKey(
                        name: "FK_UserFavorites_Listings_FavoriteListingsId",
                        column: x => x.FavoriteListingsId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Users_FavoritedById",
                        column: x => x.FavoritedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_FavoritedById",
                table: "UserFavorites",
                column: "FavoritedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavorites");
        }
    }
}
