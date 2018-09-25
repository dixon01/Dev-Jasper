namespace Luminator.AdhocMessaging.Interfaces
{
    public interface IAdhocMessageFactory
    {
        IAdhocManager CreateAdhocManager(AdhocConfiguration ac);

        IAdhocManager CreateAdhocManager(string serverUrl);
    }
}