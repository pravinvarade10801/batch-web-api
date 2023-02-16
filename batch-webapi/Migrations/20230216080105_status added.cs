using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace batchwebapi.Migrations
{
    /// <inheritdoc />
    public partial class statusadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Batch");
        }
    }
}
