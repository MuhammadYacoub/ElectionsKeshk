using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedVotingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CandidateNumber",
                table: "Candidates",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidateNumber",
                table: "Candidates");
        }
    }
}
