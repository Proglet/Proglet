using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class FixTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_DockerTestImage_DockerTestImageId",
                table: "Exercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DockerTestImage",
                table: "DockerTestImage");

            migrationBuilder.RenameTable(
                name: "DockerTestImage",
                newName: "DockerTestImages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DockerTestImages",
                table: "DockerTestImages",
                column: "DockerTestImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_DockerTestImages_DockerTestImageId",
                table: "Exercises",
                column: "DockerTestImageId",
                principalTable: "DockerTestImages",
                principalColumn: "DockerTestImageId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_DockerTestImages_DockerTestImageId",
                table: "Exercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DockerTestImages",
                table: "DockerTestImages");

            migrationBuilder.RenameTable(
                name: "DockerTestImages",
                newName: "DockerTestImage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DockerTestImage",
                table: "DockerTestImage",
                column: "DockerTestImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_DockerTestImage_DockerTestImageId",
                table: "Exercises",
                column: "DockerTestImageId",
                principalTable: "DockerTestImage",
                principalColumn: "DockerTestImageId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
