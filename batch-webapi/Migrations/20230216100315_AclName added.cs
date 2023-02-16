using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace batchwebapi.Migrations
{
    /// <inheritdoc />
    public partial class AclNameadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AclName",
                table: "ACL",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AclName",
                table: "ACL");
        }
    }
}
