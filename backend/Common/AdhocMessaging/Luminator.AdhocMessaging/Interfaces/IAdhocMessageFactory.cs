namespace Luminator.AdhocMessaging.Interfaces
{
    using System;

    public interface IAdhocMessageFactory
    {
        IAdhocManager CreateAdhocManager(AdhocConfiguration ac);

        IAdhocManager CreateAdhocManager(string serverUrl);

        IAdhocManager CreateAdhocManager(Uri adHocServerUri, Uri destinationApiUri, int apiTimeout);
    }
}