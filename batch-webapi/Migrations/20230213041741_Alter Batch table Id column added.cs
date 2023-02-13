using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace batchwebapi.Migrations
{
    /// <inheritdoc />
    public partial class AlterBatchtableIdcolumnadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Batch",
                table: "Batch");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Batch",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Batch",
                table: "Batch",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Batch",
                table: "Batch");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Batch");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Batch",
                table: "Batch",
                column: "BatchId");
        }
    }
}
