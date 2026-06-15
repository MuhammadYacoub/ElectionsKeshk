using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedVotingSystem.Migrations
{
    /// <inheritdoc />
    public partial class LinkUserToElection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ElectionId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ElectionId",
                table: "AspNetUsers",
                column: "ElectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Elections_ElectionId",
                table: "AspNetUsers",
                column: "ElectionId",
                principalTable: "Elections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Elections_ElectionId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ElectionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ElectionId",
                table: "AspNetUsers");
        }
    }
}
