using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedVotingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVoterAndCommitteeModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessCode",
                table: "Committees");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Voters",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "Voters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "Voters");

            migrationBuilder.AddColumn<string>(
                name: "AccessCode",
                table: "Committees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
