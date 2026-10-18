using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillSnap.Server.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_PortfolioUsers_PortfolioUserId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_PortfolioUsers_PortfolioUserId",
                table: "Skills");

            migrationBuilder.DropTable(
                name: "PortfolioUsers");

            migrationBuilder.DropIndex(
                name: "IX_Skills_PortfolioUserId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Projects_PortfolioUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PortfolioUserId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "PortfolioUserId",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Skills",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_ApplicationUserId",
                table: "Skills",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ApplicationUserId",
                table: "Projects",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_ApplicationUserId",
                table: "Projects",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_AspNetUsers_ApplicationUserId",
                table: "Skills",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_ApplicationUserId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_AspNetUsers_ApplicationUserId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_ApplicationUserId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ApplicationUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "PortfolioUserId",
                table: "Skills",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PortfolioUserId",
                table: "Projects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PortfolioUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Bio = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skills_PortfolioUserId",
                table: "Skills",
                column: "PortfolioUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PortfolioUserId",
                table: "Projects",
                column: "PortfolioUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_PortfolioUsers_PortfolioUserId",
                table: "Projects",
                column: "PortfolioUserId",
                principalTable: "PortfolioUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_PortfolioUsers_PortfolioUserId",
                table: "Skills",
                column: "PortfolioUserId",
                principalTable: "PortfolioUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
