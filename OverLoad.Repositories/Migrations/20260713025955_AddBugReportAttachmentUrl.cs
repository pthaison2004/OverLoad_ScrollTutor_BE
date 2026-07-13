using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverLoad.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddBugReportAttachmentUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "BugReports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "BugReports");
        }
    }
}
