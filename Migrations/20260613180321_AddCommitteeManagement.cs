using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedVotingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCommitteeManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeadName",
                table: "Committees");

            migrationBuilder.DropColumn(
                name: "Supervisor1",
                table: "Committees");

            migrationBuilder.DropColumn(
                name: "Supervisor2",
                table: "Committees");

            migrationBuilder.AddColumn<string>(
                name: "AccessCode",
                table: "Committees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadId",
                table: "Committees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommitteeId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Committees_HeadId",
                table: "Committees",
                column: "HeadId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CommitteeId",
                table: "AspNetUsers",
                column: "CommitteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Committees_CommitteeId",
                table: "AspNetUsers",
                column: "CommitteeId",
                principalTable: "Committees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Committees_AspNetUsers_HeadId",
                table: "Committees",
                column: "HeadId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Committees_CommitteeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Committees_AspNetUsers_HeadId",
                table: "Committees");

            migrationBuilder.DropIndex(
                name: "IX_Committees_HeadId",
                table: "Committees");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CommitteeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AccessCode",
                table: "Committees");

            migrationBuilder.DropColumn(
                name: "HeadId",
                table: "Committees");

            migrationBuilder.DropColumn(
                name: "CommitteeId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "HeadName",
                table: "Committees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supervisor1",
                table: "Committees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supervisor2",
                table: "Committees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
