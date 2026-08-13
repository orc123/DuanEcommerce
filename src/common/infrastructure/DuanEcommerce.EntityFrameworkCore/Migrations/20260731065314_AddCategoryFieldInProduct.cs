using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuanEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryFieldInProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "AppProducts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CategorySlug",
                table: "AppProducts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "CategorySlug",
                table: "AppProducts");
        }
    }
}
