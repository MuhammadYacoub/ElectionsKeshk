using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedVotingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddElectionStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_ElectionCategories_CategoryId",
                table: "Candidates");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Candidates",
                newName: "PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_Candidates_CategoryId",
                table: "Candidates",
                newName: "IX_Candidates_PositionId");

            migrationBuilder.AddColumn<string>(
                name: "BylawPdfPath",
                table: "Elections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ElectionCategoryId",
                table: "Candidates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ElectoralSymbol",
                table: "Candidates",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequiredCount = table.Column<int>(type: "int", nullable: false),
                    ElectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_ElectionCategoryId",
                table: "Candidates",
                column: "ElectionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_ElectionId",
                table: "Positions",
                column: "ElectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_ElectionCategories_ElectionCategoryId",
                table: "Candidates",
                column: "ElectionCategoryId",
                principalTable: "ElectionCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Positions_PositionId",
                table: "Candidates",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_ElectionCategories_ElectionCategoryId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Positions_PositionId",
                table: "Candidates");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_ElectionCategoryId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "BylawPdfPath",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "ElectionCategoryId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "ElectoralSymbol",
                table: "Candidates");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "Candidates",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Candidates_PositionId",
                table: "Candidates",
                newName: "IX_Candidates_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_ElectionCategories_CategoryId",
                table: "Candidates",
                column: "CategoryId",
                principalTable: "ElectionCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
