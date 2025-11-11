using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectEcomerceFinal.Data.Migrations
{
    /// <inheritdoc />
    public partial class ispaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPaid",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPaid",
                table: "Order");
        }
    }
}
