using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNUL.Shared.Migrations
{
    /// <inheritdoc />
    public partial class BilingualHelpContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "HelpCategories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyAr",
                table: "HelpArticles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "HelpArticles",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerAr",
                table: "FAQItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionAr",
                table: "FAQItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "HelpCategories");

            migrationBuilder.DropColumn(
                name: "BodyAr",
                table: "HelpArticles");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "HelpArticles");

            migrationBuilder.DropColumn(
                name: "AnswerAr",
                table: "FAQItems");

            migrationBuilder.DropColumn(
                name: "QuestionAr",
                table: "FAQItems");
        }
    }
}
