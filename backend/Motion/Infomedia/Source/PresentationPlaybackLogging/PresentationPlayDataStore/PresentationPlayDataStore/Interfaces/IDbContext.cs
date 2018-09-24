namespace Luminator.PresentationPlayLogging.DataStore.Interfaces
{
    public interface IDbContext
    {
        /// <summary>Gets the Sql connection string.</summary>
        string ConnectionString { get; }
    }
}