using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SlaveManagers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Url = table.Column<string>(type: "varchar(64)", nullable: true),
                    Auth = table.Column<string>(type: "varchar(64)", nullable: true),
                    Enabled = table.Column<ulong>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaveManagers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(64)", nullable: true),
                    Email = table.Column<string>(type: "varchar(64)", nullable: true),
                    OrganizationIdentifier = table.Column<string>(type: "varchar(64)", nullable: true),
                    UserRole = table.Column<int>(nullable: false, defaultValue: 0),
                    RegistrationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "CourseTemplates",
                columns: table => new
                {
                    CourseTemplateId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false),
                    Title = table.Column<string>(type: "varchar(64)", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GitUrl = table.Column<string>(type: "varchar(255)", nullable: false),
                    DockerRefreshImage = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedByUserId = table.Column<int>(nullable: true),
                    MaterialUrl = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTemplates", x => x.CourseTemplateId);
                    table.ForeignKey(
                        name: "FK_CourseTemplates_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OauthLogins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OauthLoginId = table.Column<int>(nullable: false),
                    LoginService = table.Column<string>(type: "varchar(64)", nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OauthLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OauthLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseTemplateId = table.Column<int>(nullable: false),
                    Enabled = table.Column<ulong>(type: "bit", nullable: false),
                    Curriculum = table.Column<string>(type: "varchar(64)", nullable: true),
                    HidderAfter = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseTemplateId);
                    table.ForeignKey(
                        name: "FK_Courses_CourseTemplates_CourseTemplateId",
                        column: x => x.CourseTemplateId,
                        principalTable: "CourseTemplates",
                        principalColumn: "CourseTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseRegistrations",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    Active = table.Column<ulong>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseRegistrations", x => new { x.UserId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_CourseRegistrations_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseTemplateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseRegistrations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    ExerciseId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CourseTemplateId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(64)", nullable: false),
                    Subject = table.Column<string>(type: "VARCHAR(64)", nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    PublishDate = table.Column<DateTime>(nullable: false),
                    Hidden = table.Column<ulong>(type: "bit", nullable: false),
                    Checksum = table.Column<string>(type: "varchar(64)", nullable: false),
                    Size = table.Column<int>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    SolutionVisableAfter = table.Column<DateTime>(nullable: false),
                    HasTests = table.Column<ulong>(type: "bit", nullable: false),
                    RunTimeParameters = table.Column<string>(nullable: true),
                    CodeReviewEnable = table.Column<ulong>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.ExerciseId);
                    table.ForeignKey(
                        name: "FK_Exercises_Courses_CourseTemplateId",
                        column: x => x.CourseTemplateId,
                        principalTable: "Courses",
                        principalColumn: "CourseTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    PointId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExerciseId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.PointId);
                    table.ForeignKey(
                        name: "FK_Points_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "ExerciseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    ExerciseId = table.Column<int>(nullable: true),
                    Processed = table.Column<ulong>(type: "bit", nullable: false),
                    SubmissionIp = table.Column<string>(type: "varchar(16)", nullable: true),
                    SubmissionTime = table.Column<DateTime>(nullable: false),
                    SubmissionZip = table.Column<byte[]>(type: "LongBlob", maxLength: 16777216, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_Submissions_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "ExerciseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseRegistrations_CourseId",
                table: "CourseRegistrations",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTemplates_CreatedByUserId",
                table: "CourseTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_CourseTemplateId",
                table: "Exercises",
                column: "CourseTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_OauthLogins_UserId",
                table: "OauthLogins",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OauthLogins_Id_LoginService",
                table: "OauthLogins",
                columns: new[] { "Id", "LoginService" });

            migrationBuilder.CreateIndex(
                name: "IX_Points_ExerciseId",
                table: "Points",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ExerciseId",
                table: "Submissions",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UserId",
                table: "Submissions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseRegistrations");

            migrationBuilder.DropTable(
                name: "OauthLogins");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "SlaveManagers");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "CourseTemplates");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
