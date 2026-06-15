using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedVotingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCooldownThreshold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CooldownThreshold",
                table: "Committees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CooldownThreshold",
                table: "Committees");
        }
    }
}
