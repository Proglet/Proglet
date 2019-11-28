using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Proglet.Core.Data;
using Proglet.Core.Data.Internal;
using System;
using System.Collections.Generic;
using System.Text;


namespace CoreDataORM
{
    public class DataContext : DbContext
    {
        /*public DataContext(DbContextOptions<AppContext> options)
            : base(options)
        { }*/

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }

        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<CourseTemplate> CourseTemplates { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        public DbSet<Point> Points { get; set; }

        public DataContext()
        {
        }

        private ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                   builder.AddConsole()
                          .AddFilter(DbLoggerCategory.Database.Command.Name,
                                     LogLevel.Information));
            return serviceCollection.BuildServiceProvider()
                    .GetService<ILoggerFactory>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;database=Proglet;user=Proglet;password=Welkom01");
            optionsBuilder.UseLoggerFactory(GetLoggerFactory());
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Username);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.UserRole).HasDefaultValue(UserRoles.User);
                entity.Property(e => e.RegistrationDate).HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);
                entity.Property(e => e.Name).IsRequired();
                entity.HasOne(e => e.CourseTemplate);
                entity.Property(c => c.CreatedOn).HasDefaultValueSql("NOW()");
                entity.HasOne(c => c.CreatedBy).WithMany(u => u.CreatedCourses);
            });

            modelBuilder.Entity<CourseTemplate>(entitiy =>
            {
                entitiy.HasKey(e => e.CourseTemplateId);
                entitiy.Property(e => e.GitUrl).IsRequired();
                entitiy.HasOne(e => e.CreatedBy);
                entitiy.Property(e => e.CreatedOn).HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.HasKey(e => e.ExerciseId);
                entity.Property(e => e.Name).IsRequired();
                entity.HasOne(d => d.Course);
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => e.SubmissionId);
                entity.Property(e => e.SubmissionTime).HasDefaultValueSql("NOW()");
                entity.HasOne(e => e.User).WithMany(e => e.Submissions);
                entity.HasOne(e => e.Exercise);
            });
        }
    }
}
