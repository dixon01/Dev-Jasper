namespace CodeFirstMigrations.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;

    using CodeFirstMigrations.Migrations.Operations;

    public static class Extensions
    {
        /// <summary>
        /// This extensions add the <see cref="EnsureTeacherOnLessonsOperation"/> custom operation.
        /// </summary>
        /// <param name="migration"></param>
        /// <param name="lessonsTable"></param>
        /// <param name="teachersTable"></param>
        public static void EnsureTeacherOnLessons(this DbMigration migration, string lessonsTable, string teachersTable)
        {
            ((IDbMigration)migration).AddOperation(new EnsureTeacherOnLessonsOperation(lessonsTable, teachersTable));
        }
    }
}