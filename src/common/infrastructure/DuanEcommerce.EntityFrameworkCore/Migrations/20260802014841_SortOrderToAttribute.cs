using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuanEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class SortOrderToAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "AppProductAttribute",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AppProductAttribute");
        }
    }
}
