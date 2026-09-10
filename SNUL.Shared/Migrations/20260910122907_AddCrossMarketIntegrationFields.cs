using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNUL.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddCrossMarketIntegrationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WelcoRfqId",
                table: "RFQs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WelcoRfqNumber",
                table: "RFQs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WelcoProductId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WelcoOrderId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WelcoOrderNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WelcoCompanyId",
                table: "Companies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WelcoCategoryId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WelcoRfqId",
                table: "RFQs");

            migrationBuilder.DropColumn(
                name: "WelcoRfqNumber",
                table: "RFQs");

            migrationBuilder.DropColumn(
                name: "WelcoProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WelcoOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WelcoOrderNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WelcoCompanyId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "WelcoCategoryId",
                table: "Categories");
        }
    }
}
