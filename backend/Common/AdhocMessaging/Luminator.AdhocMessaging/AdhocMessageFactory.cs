namespace Luminator.AdhocMessaging
{
    using System;
    using System.ComponentModel;

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
   
        public IAdhocManager CreateAdhocManager(Uri adHocUri, Uri destinationsApiUri, int apiTimeout = AdhocConstants.DefaultHttpClientTimeoutInMilliseconds)
        {
            var config = new AdhocConfiguration
                             {
                                 HttpClientTimeoutInMilliseconds = apiTimeout,
                                 MessageApiPort = adHocUri.Port.ToString(),
                                 MessageApiBaseUrl = adHocUri.OriginalString,
                                 DestinationsApiUrl = destinationsApiUri.OriginalString
            };

            return new AdhocManager(config);
        }        
    }
}