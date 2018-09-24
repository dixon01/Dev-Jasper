namespace CodeFirstMigrations.Migrations.Operations
{
    using System.Data.Entity.Migrations.Model;

    public class EnsureTeacherOnLessonsOperation : MigrationOperation
    {
        public EnsureTeacherOnLessonsOperation(string lessonsTable, string teachersTable)
            : base(null)
        {
            this.LessonsTable = lessonsTable;
            this.TeachersTable = teachersTable;
        }

        public string LessonsTable { get; set; }

        public string TeachersTable { get; set; }

        public override bool IsDestructiveChange
        {
            get
            {
                return false;
            }
        }
    }
}