using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Proglet.Core.Data;
using Proglet.Core.Data.Internal;
using System;
using System.Text;
using System.Linq;

namespace CoreDataORM
{
    class Program
    {
        static void Main(string[] args)
        {
            //DeleteData();
            //InsertData();
            PrintData();
            Console.Read();
        }

        private static void DeleteData()
        {
            using (var context = new DataContext())
            {
                context.Database.EnsureDeleted();
            }
        }

        private static void InsertData()
        {
            using (var context = new DataContext())
            {
                // Creates the database if not exists
                context.Database.EnsureCreated();

                // Adds a publisher
                var user = new User
                {
                    Username = "bpssoft",
                    Email = "psm.goossens@avans.nl",
                    OrganizationIdentifier = "12345",
                    UserRole = UserRoles.Administrator,
                };
                context.Users.Add(user);

                var template = new CourseTemplate
                {
                    CreatedBy = user,
                    GitUrl = "http://www.github.com",
                    CreatedOn = DateTime.Now,

                };
                context.CourseTemplates.Add(template);

                // Insert Courses
                var ogp0 = new Course
                {
                    Name = "OGP0",
                    Description = "Object Georienteerd Programmeren 0",
                    Enabled = true,
                    MaterialUrl = "http://www.google.nl",
                    HidderAfter = DateTime.Now,
                    Title = "OGP0",
                    CourseTemplate = template,
                    CreatedBy = user,
                };
                context.Courses.Add(ogp0);

                var ogp1 = new Course
                {
                    Name = "OGP1",
                    Description = "Object Georienteerd Programmeren 1",
                    Enabled = true,
                    MaterialUrl = "http://www.google.nl",
                    HidderAfter = null,
                    Title = "OGP1",
                    CourseTemplate = template,
                    CreatedBy = user,
                };
                context.Courses.Add(ogp1);



                var ex = new Exercise
                {
                    Course = ogp0,
                    Name = "Test",
                    HasTests = false,
                    Hidden = false,
                    UpdatedAt = DateTime.Now,
                    
                };
                context.Exercises.Add(ex);


                var submission = new Submission
                {
                    Exercise = ex,
                    User  = user,
                    Processed = false,
                };
                context.Submissions.Add(submission);


                // Saves changes
                
                context.SaveChanges();
            }
        }

        private static void PrintData()
        {
            // Gets and prints all books in database
            using (var context = new DataContext())
            {
                var result = context.Courses.Select(c =>
                new
                {
                    Course = c,
                    CreatedBy = c.CreatedBy,
                    CourseTemplate = c.CourseTemplate,
                }).ToList();


                var courses = context.Courses.Include(c => c.CreatedBy).Include(c => c.CourseTemplate);
                
                foreach (var course in courses)
                {
                    var data = new StringBuilder();
                    data.AppendLine($"Name: {course.Name}");
                    data.AppendLine($"Title: {course.Title}");
                    data.AppendLine($"Material Url: {course.MaterialUrl}");
                    data.AppendLine($"Hidden after: {course.HidderAfter}");
                    data.AppendLine($"Created by: {course.CreatedBy.Username}");
                    Console.WriteLine(data.ToString());
                }



            }
        }
    }
}
