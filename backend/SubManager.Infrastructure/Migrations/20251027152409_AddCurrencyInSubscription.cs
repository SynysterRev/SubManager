using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyInSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Subscriptions",
                type: "text",
                nullable: false,
                defaultValue: "EUR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Subscriptions");
        }
    }
}
