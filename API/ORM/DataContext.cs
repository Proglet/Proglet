using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Proglet.Core.Data;
using Proglet.Core.Data.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ORM
{
    public class DataContext : DbContext
    {
        static private DatabaseConfig config; //this is dirty, but it makes it so that we can call the constructor without parameters with the proper config

        /*public DataContext(DbContextOptions<AppContext> options)
: base(options)
{ }*/

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseRegistration> CourseRegistrations { get; set; }

        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<CourseTemplate> CourseTemplates { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        public DbSet<Point> Points { get; set; }
        public DbSet<OauthLogin> OauthLogins { get; set; }
        public DbSet<SlaveManager> SlaveManagers { get; set; }

        public DbSet<TestResult> TestResults { get; set; }

        public DbSet<Test> Tests { get; set; }

        public DataContext(IOptions<DatabaseConfig> config)
        {
            if (config != null)
                DataContext.config = config.Value;
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
            optionsBuilder.UseMySql(config.configstring);
            optionsBuilder.UseLoggerFactory(GetLoggerFactory());

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(user =>
            {
                user.Property(u => u.UserRole).HasDefaultValue(UserRoles.Student);
                //                entity.Property(e => e.RegistrationDate).HasDefaultValueSql("NOW()");
                user.HasOne(u => u.OauthLogin).WithOne(o => o.User);
                user.HasMany(u => u.CourseRegistrations).WithOne(r => r.User);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseTemplateId);
                entity.HasOne(e => e.CourseTemplate);
            });

            modelBuilder.Entity<CourseRegistration>(reg =>
            {
                reg.HasKey(r => new { r.UserId, r.CourseId });
                reg.HasOne(r => r.Course).WithMany(c => c.Users);
                reg.HasOne(r => r.User).WithMany(u => u.CourseRegistrations);
            });

            modelBuilder.Entity<CourseTemplate>(entity =>
            {
                entity.HasKey(e => e.CourseTemplateId);
                //entity.Property(e => e.GitUrl).IsRequired();
                //entity.Property(e => e.Name).IsRequired();
                entity.HasOne(e => e.CreatedBy);
                //entity.Property(e => e.CreatedOn).HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.HasKey(e => e.ExerciseId);
                //entity.Property(e => e.Name).IsRequired();
                entity.HasOne(d => d.CourseTemplate);
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => e.SubmissionId);
                //entity.Property(e => e.SubmissionTime).HasDefaultValueSql("NOW()");
                entity.HasOne(e => e.User).WithMany(e => e.Submissions);
                entity.HasOne(e => e.Exercise);
            });

            modelBuilder.Entity<OauthLogin>(entity =>
            {
                entity.HasIndex(e => new { e.Id, e.LoginService });
                //entity.Property(e => e.LoginService).HasColumnType("varchar(64)");
                entity.HasOne(e => e.User).WithOne(e => e.OauthLogin);
            });

            modelBuilder.Entity<SlaveManager>(entity =>
            {

            });

        }
    }
}
