using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SubManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Subscriptions");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Streaming" },
                    { 2, "Music" },
                    { 3, "Cloud Storage" },
                    { 4, "Productivity" },
                    { 5, "Fitness" },
                    { 6, "Gaming" },
                    { 7, "Development" },
                    { 8, "Education" },
                    { 9, "Communication" },
                    { 10, "news_media" },
                    { 11, "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CategoryId",
                table: "Subscriptions",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Categories_CategoryId",
                table: "Subscriptions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Categories_CategoryId",
                table: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_CategoryId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Subscriptions");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Subscriptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
