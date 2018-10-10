namespace Luminator.AdhocMessaging
{
    using Luminator.AdhocMessaging.Interfaces;

    public class AdhocMessageFactory : IAdhocMessageFactory
    {
        public IAdhocManager CreateAdhocManager(AdhocConfiguration ac)
        {
            return new AdhocManager(ac);
        }

        public IAdhocManager CreateAdhocManager(string serverUrl)
        {
            return new AdhocManager(serverUrl);
        }
    }
}