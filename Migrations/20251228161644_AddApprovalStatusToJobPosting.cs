using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalStatusToJobPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "JobPostings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "JobPostings");
        }
    }
}
