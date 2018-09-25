namespace CodeFirstMigrations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using CodeFirstMigrations.Migrations;

    class Program
    {
        /// <summary>
        /// Usage: comment code specific for each migration before moving to next one.
        /// Example:
        /// - Run the application
        /// - Comment lines under 'First migration' in Program only
        /// - Uncomment lines under 'Second migration' AND do the same for DataContext and entities (clases and
        ///   properties)
        /// - Add migration
        /// - Run the application
        /// - Repeat for next migrations..
        /// Important: for the 'Third migration', add the following code in the Up method of the migration before the
        /// line with the 'AddForeignKey' invocation
        /// this.EnsureTeacherOnLessons("dbo.Lessons", "dbo.Teachers");
        /// Suggested names for migrations:
        /// - First migration: Setup
        /// - Second migration: Students
        /// - Third migration: Teachers
        /// - Fourth migration: StudentDetails
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Database.SetInitializer(new NullDatabaseInitializer<DataContext>());
                new DbMigrator(new Configuration()).Update();
                using (var dataContext = new DataContext())
                {
                    // First migration
                    var lesson = new Lesson { Title = "Lesson1" };
                    dataContext.Lessons.Add(lesson);

                    // Second migration
                    ////var student = new Student { Name = "Student1" };
                    ////dataContext.Students.Add(student);
                    ////var lesson = dataContext.Lessons.First();
                    ////lesson.Students.Add(student);
                    
                    // Third migration
                    ////var teacher = new Teacher { Name = "Teacher1" };
                    ////dataContext.Teachers.Add(teacher);

                    // Fourth migration
                    // Nothing special to do, just check that the database has the new column
                    dataContext.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception: {0}", exception);
                Console.ResetColor();
            }

            Console.WriteLine("Completed. Type <Enter> to exit");
            Console.ReadKey();
        }
    }

    class DataContext : DbContext
    {
        // First migration
        public IDbSet<Lesson> Lessons { get; set; }

        // Second migration
        ////public IDbSet<Student> Students { get; set; }

        // Third migration
        ////public IDbSet<Teacher> Teachers { get; set; }
    }

    class Lesson
    {
        public Lesson()
        {
            // Second migration
            ////this.Students = new Collection<Student>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        // Second migration
        ////public ICollection<Student> Students { get; set; }

        // Third migration
        ////[Required]
        ////public Teacher Teacher { get; set; }
    }

    // Second migration
    ////class Student
    ////{
    ////    public Student()
    ////    {
    ////        this.Lessons = new Collection<Lesson>();
    ////    }

    ////    public int Id { get; set; }

    ////    public string Name { get; set; }

    ////    public ICollection<Lesson> Lessons { get; set; }

    ////    // Fourth migration
    ////    ////public string LastName { get; set; }
    ////}

    // Third migration
    ////class Teacher
    ////{
    ////    public Teacher()
    ////    {
    ////        this.Lessons = new Collection<Lesson>();
    ////    }

    ////    public int Id { get; set; }

    ////    public ICollection<Lesson> Lessons { get; set; }

    ////    public string Name { get; set; }
    ////}
}