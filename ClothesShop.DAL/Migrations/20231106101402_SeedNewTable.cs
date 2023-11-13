using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothesShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedNewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WishlistId",
                table: "ProductImages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ProductClothes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_WishlistId",
                table: "ProductImages",
                column: "WishlistId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductClothes_ProductId",
                table: "ProductClothes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_ApplicationUserId",
                table: "Wishlists",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductClothes_Wishlists_ProductId",
                table: "ProductClothes",
                column: "ProductId",
                principalTable: "Wishlists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Wishlists_WishlistId",
                table: "ProductImages",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductClothes_Wishlists_ProductId",
                table: "ProductClothes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Wishlists_WishlistId",
                table: "ProductImages");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_WishlistId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductClothes_ProductId",
                table: "ProductClothes");

            migrationBuilder.DropColumn(
                name: "WishlistId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductClothes");
        }
    }
}
