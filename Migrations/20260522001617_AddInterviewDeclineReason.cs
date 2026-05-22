using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitAPP.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewDeclineReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeclineReason",
                table: "Entretiens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeclineReason",
                table: "Entretiens");
        }
    }
}
