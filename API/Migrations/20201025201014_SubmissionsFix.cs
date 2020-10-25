using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class SubmissionsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Submissions_SubmissionId",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "Processed",
                table: "Submissions");

            migrationBuilder.AlterColumn<int>(
                name: "SubmissionId",
                table: "TestResults",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Submissions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "Submissions",
                type: "varchar(64)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Submissions",
                type: "enum(Unprocessed,Processing,Processed)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_CourseId",
                table: "Submissions",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Courses_CourseId",
                table: "Submissions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseTemplateId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Submissions_SubmissionId",
                table: "TestResults",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "SubmissionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Courses_CourseId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Submissions_SubmissionId",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_CourseId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Submissions");

            migrationBuilder.AlterColumn<int>(
                name: "SubmissionId",
                table: "TestResults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<ulong>(
                name: "Processed",
                table: "Submissions",
                type: "bit",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Submissions_SubmissionId",
                table: "TestResults",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "SubmissionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
