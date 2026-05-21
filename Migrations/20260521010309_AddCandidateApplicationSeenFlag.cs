using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitAPP.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateApplicationSeenFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSeenByCandidat",
                table: "Candidatures",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSeenByCandidat",
                table: "Candidatures");
        }
    }
}
