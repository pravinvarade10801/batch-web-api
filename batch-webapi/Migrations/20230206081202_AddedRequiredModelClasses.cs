using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace batch_webapi.Migrations
{
    public partial class AddedRequiredModelClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ACL",
                columns: table => new
                {
                    AclId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACL", x => x.AclId);
                });

            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    AttributesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.AttributesId);
                });

            migrationBuilder.CreateTable(
                name: "ReadGroups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AclId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadGroups", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_ReadGroups_ACL_AclId",
                        column: x => x.AclId,
                        principalTable: "ACL",
                        principalColumn: "AclId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReadUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AclId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_ReadUsers_ACL_AclId",
                        column: x => x.AclId,
                        principalTable: "ACL",
                        principalColumn: "AclId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Batch",
                columns: table => new
                {
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AclId = table.Column<int>(type: "int", nullable: false),
                    AttributesId = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batch", x => x.BatchId);
                    table.ForeignKey(
                        name: "FK_Batch_ACL_AclId",
                        column: x => x.AclId,
                        principalTable: "ACL",
                        principalColumn: "AclId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Batch_Attributes_AttributesId",
                        column: x => x.AttributesId,
                        principalTable: "Attributes",
                        principalColumn: "AttributesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batch_AclId",
                table: "Batch",
                column: "AclId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Batch_AttributesId",
                table: "Batch",
                column: "AttributesId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReadGroups_AclId",
                table: "ReadGroups",
                column: "AclId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadUsers_AclId",
                table: "ReadUsers",
                column: "AclId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Batch");

            migrationBuilder.DropTable(
                name: "ReadGroups");

            migrationBuilder.DropTable(
                name: "ReadUsers");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropTable(
                name: "ACL");
        }
    }
}
