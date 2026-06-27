using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverLoad.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddAiLimitsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AiQuestionsAskedToday",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAiQuestionDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiQuestionsAskedToday",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastAiQuestionDate",
                table: "Users");
        }
    }
}
