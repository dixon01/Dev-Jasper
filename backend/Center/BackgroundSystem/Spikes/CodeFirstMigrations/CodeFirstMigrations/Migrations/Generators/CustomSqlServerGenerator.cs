namespace CodeFirstMigrations.Migrations.Generators
{
    using System;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.SqlServer;

    using CodeFirstMigrations.Migrations.Operations;

    /// <summary>
    /// This generator handles custom operations and ensures that the correct sql statements are added to the migration.
    /// </summary>
    class CustomSqlServerGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void Generate(MigrationOperation migrationOperation)
        {
            var ensureTeacherOnLessonOperation = migrationOperation as EnsureTeacherOnLessonsOperation;
            if (ensureTeacherOnLessonOperation == null)
            {
                return;
            }

            var lessonsCountVariableName = "lessonsCount_" + Guid.NewGuid().ToString("N");
            var teacherIdVariableName = "teacherId_" + Guid.NewGuid().ToString("N");

            const string SqlFormat = @"
DECLARE @{0} int
SELECT @{0} = COUNT(*) FROM {1}
IF @{0} > 0
    BEGIN
        DECLARE @{2} int = NULL
        SELECT TOP 1 @{2} = [Id]
        FROM {3}

        IF @{2} IS NULL
            BEGIN
                INSERT INTO {3}
                    ([Name])
                    VALUES
                    ('Default Teacher')
                SET @{2} = SCOPE_IDENTITY()
                    
            END
        UPDATE {1}
        SET [Teacher_Id] = @{2}
    END
";
            var sql = string.Format(
                SqlFormat,
                lessonsCountVariableName,
                ensureTeacherOnLessonOperation.LessonsTable,
                teacherIdVariableName,
                ensureTeacherOnLessonOperation.TeachersTable);
            this.Statement(sql);
        }
    }
}