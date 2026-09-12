using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNUL.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddWelcoProviderMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WelcoProviderMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WelcoProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    System = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WelcoProviderMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WelcoProviderMaps_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WelcoProviderMaps_CompanyId",
                table: "WelcoProviderMaps",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_WelcoProviderMaps_WelcoProviderId_System",
                table: "WelcoProviderMaps",
                columns: new[] { "WelcoProviderId", "System" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WelcoProviderMaps");
        }
    }
}
