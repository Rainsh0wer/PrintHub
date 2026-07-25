using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrintHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddImageFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrls",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Machines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "DocumentFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShopGalleryImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopGalleryImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopGalleryImages_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopGalleryImages_ShopId",
                table: "ShopGalleryImages",
                column: "ShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopGalleryImages");

            migrationBuilder.DropColumn(
                name: "PhotoUrls",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Machines");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "DocumentFiles");
        }
    }
}
