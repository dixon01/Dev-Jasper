namespace Luminator.PresentationPlayLogging.DataStore
{
    using System.Data.Entity;

    public class PresentationPlayLoggingCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<DbContext>
    {
        protected override void Seed(DbContext context)
        {
            base.Seed(context);
        }
    }
}