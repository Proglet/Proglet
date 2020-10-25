﻿// <auto-generated />
using System;
using API.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Proglet.Core.Data.Course", b =>
                {
                    b.Property<int>("CourseTemplateId")
                        .HasColumnType("int");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("Curriculum")
                        .HasColumnType("varchar(64)");

                    b.Property<ulong>("Enabled")
                        .HasColumnType("bit");

                    b.Property<DateTime>("HidderAfter")
                        .HasColumnType("DATE");

                    b.HasKey("CourseTemplateId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("Proglet.Core.Data.CourseRegistration", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<ulong>("Active")
                        .HasColumnType("bit");

                    b.HasKey("UserId", "CourseId");

                    b.HasIndex("CourseId");

                    b.ToTable("CourseRegistrations");
                });

            modelBuilder.Entity("Proglet.Core.Data.DockerTestImage", b =>
                {
                    b.Property<int>("DockerTestImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ImageName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("DockerTestImageId");

                    b.ToTable("DockerTestImages");
                });

            modelBuilder.Entity("Proglet.Core.Data.Exercise", b =>
                {
                    b.Property<int>("ExerciseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Checksum")
                        .IsRequired()
                        .HasColumnType("varchar(64)");

                    b.Property<ulong>("CodeReviewEnable")
                        .HasColumnType("bit");

                    b.Property<int>("CourseTemplateId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DockerTestImageId")
                        .HasColumnType("int");

                    b.Property<ulong>("HasTests")
                        .HasColumnType("bit");

                    b.Property<ulong>("Hidden")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("VARCHAR(64)");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("RunTimeParameters")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<DateTime>("SolutionVisableAfter")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("VARCHAR(64)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("ExerciseId");

                    b.HasIndex("CourseTemplateId");

                    b.HasIndex("DockerTestImageId");

                    b.ToTable("Exercises");
                });

            modelBuilder.Entity("Proglet.Core.Data.Internal.CourseTemplate", b =>
                {
                    b.Property<int>("CourseTemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DockerRefreshImage")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("GitUrl")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("MaterialUrl")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(64)");

                    b.Property<string>("Title")
                        .HasColumnType("varchar(64)");

                    b.HasKey("CourseTemplateId");

                    b.HasIndex("CreatedByUserId");

                    b.ToTable("CourseTemplates");
                });

            modelBuilder.Entity("Proglet.Core.Data.OauthLogin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("LoginService")
                        .HasColumnType("varchar(64)");

                    b.Property<int>("OauthLoginId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.HasIndex("Id", "LoginService");

                    b.ToTable("OauthLogins");
                });

            modelBuilder.Entity("Proglet.Core.Data.Point", b =>
                {
                    b.Property<int>("PointId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("ExerciseId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("PointId");

                    b.HasIndex("ExerciseId");

                    b.ToTable("Points");
                });

            modelBuilder.Entity("Proglet.Core.Data.SlaveManager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Auth")
                        .HasColumnType("varchar(64)");

                    b.Property<ulong>("Enabled")
                        .HasColumnType("bit");

                    b.Property<string>("Url")
                        .HasColumnType("varchar(64)");

                    b.HasKey("Id");

                    b.ToTable("SlaveManagers");
                });

            modelBuilder.Entity("Proglet.Core.Data.Submission", b =>
                {
                    b.Property<int>("SubmissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<int?>("ExerciseId")
                        .HasColumnType("int");

                    b.Property<string>("JobId")
                        .HasColumnType("varchar(64)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("enum(Unprocessed,Processing,Processed)");

                    b.Property<string>("SubmissionIp")
                        .HasColumnType("varchar(16)");

                    b.Property<DateTime>("SubmissionTime")
                        .HasColumnType("datetime(6)");

                    b.Property<byte[]>("SubmissionZip")
                        .HasColumnType("LongBlob")
                        .HasMaxLength(16777216);

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("SubmissionId");

                    b.HasIndex("CourseId");

                    b.HasIndex("ExerciseId");

                    b.HasIndex("UserId");

                    b.ToTable("Submissions");
                });

            modelBuilder.Entity("Proglet.Core.Data.Test", b =>
                {
                    b.Property<int>("TestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClassName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("PointId")
                        .HasColumnType("int");

                    b.HasKey("TestId");

                    b.HasIndex("PointId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("Proglet.Core.Data.TestResult", b =>
                {
                    b.Property<int>("TestResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Pass")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("StackTrace")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("SubmissionId")
                        .HasColumnType("int");

                    b.Property<int?>("TestId")
                        .HasColumnType("int");

                    b.HasKey("TestResultId");

                    b.HasIndex("SubmissionId");

                    b.HasIndex("TestId");

                    b.ToTable("TestResults");
                });

            modelBuilder.Entity("Proglet.Core.Data.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(64)");

                    b.Property<string>("FullName")
                        .HasColumnType("varchar(64)");

                    b.Property<string>("OrganizationIdentifier")
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserRole")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Username")
                        .HasColumnType("varchar(64)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Proglet.Core.Data.Course", b =>
                {
                    b.HasOne("Proglet.Core.Data.Internal.CourseTemplate", "CourseTemplate")
                        .WithMany()
                        .HasForeignKey("CourseTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Proglet.Core.Data.CourseRegistration", b =>
                {
                    b.HasOne("Proglet.Core.Data.Course", "Course")
                        .WithMany("Users")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Proglet.Core.Data.User", "User")
                        .WithMany("CourseRegistrations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Proglet.Core.Data.Exercise", b =>
                {
                    b.HasOne("Proglet.Core.Data.Course", "CourseTemplate")
                        .WithMany()
                        .HasForeignKey("CourseTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Proglet.Core.Data.DockerTestImage", "DockerTestImage")
                        .WithMany()
                        .HasForeignKey("DockerTestImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Proglet.Core.Data.Internal.CourseTemplate", b =>
                {
                    b.HasOne("Proglet.Core.Data.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");
                });

            modelBuilder.Entity("Proglet.Core.Data.OauthLogin", b =>
                {
                    b.HasOne("Proglet.Core.Data.User", "User")
                        .WithOne("OauthLogin")
                        .HasForeignKey("Proglet.Core.Data.OauthLogin", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Proglet.Core.Data.Point", b =>
                {
                    b.HasOne("Proglet.Core.Data.Exercise", "Exercise")
                        .WithMany("Points")
                        .HasForeignKey("ExerciseId");
                });

            modelBuilder.Entity("Proglet.Core.Data.Submission", b =>
                {
                    b.HasOne("Proglet.Core.Data.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Proglet.Core.Data.Exercise", "Exercise")
                        .WithMany()
                        .HasForeignKey("ExerciseId");

                    b.HasOne("Proglet.Core.Data.User", "User")
                        .WithMany("Submissions")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Proglet.Core.Data.Test", b =>
                {
                    b.HasOne("Proglet.Core.Data.Point", "Point")
                        .WithMany("Tests")
                        .HasForeignKey("PointId");
                });

            modelBuilder.Entity("Proglet.Core.Data.TestResult", b =>
                {
                    b.HasOne("Proglet.Core.Data.Submission", "Submission")
                        .WithMany("TestResults")
                        .HasForeignKey("SubmissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Proglet.Core.Data.Test", "Test")
                        .WithMany()
                        .HasForeignKey("TestId");
                });
#pragma warning restore 612, 618
        }
    }
}
