using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class DockerTestImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DockerTestImageId",
                table: "Exercises",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DockerTestImage",
                columns: table => new
                {
                    DockerTestImageId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ImageName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DockerTestImage", x => x.DockerTestImageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_DockerTestImageId",
                table: "Exercises",
                column: "DockerTestImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_DockerTestImage_DockerTestImageId",
                table: "Exercises",
                column: "DockerTestImageId",
                principalTable: "DockerTestImage",
                principalColumn: "DockerTestImageId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_DockerTestImage_DockerTestImageId",
                table: "Exercises");

            migrationBuilder.DropTable(
                name: "DockerTestImage");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_DockerTestImageId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "DockerTestImageId",
                table: "Exercises");
        }
    }
}
