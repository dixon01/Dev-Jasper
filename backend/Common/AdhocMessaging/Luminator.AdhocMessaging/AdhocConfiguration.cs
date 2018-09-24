namespace Luminator.AdhocMessaging
{
    using System;

    public class AdhocConfiguration
    {
        public AdhocConfiguration(
            string destinationsApiAddress = AdhocConstants.DefaultDestinationsApiUrl, 
            string port = AdhocConstants.DefaultHttpPort, 
            string messageApiUrl = AdhocConstants.DefaultMessageApiUrl, 
            string messageApiPort = AdhocConstants.DefaultHttpPort, 
            int httpTimeOut = AdhocConstants.DefaultHttpClientTimeoutInMilliseconds, 
            string tenentId = AdhocConstants.DefaultTenentId)
        {
            if (destinationsApiAddress.Contains("http://localhost:") && string.IsNullOrEmpty(port))
            {
                var uri = new Uri(destinationsApiAddress);
                this.Port = uri.GetComponents(UriComponents.Port, UriFormat.Unescaped);
                this.DestinationsApiUrl = uri.GetComponents( UriComponents.AbsoluteUri & ~UriComponents.Port,UriFormat.UriEscaped).TrimEnd('/');
            }
            else if (destinationsApiAddress.Equals("http://localhost") && string.IsNullOrEmpty(port))
            {
                this.DestinationsApiUrl = destinationsApiAddress;
                this.Port = AdhocConstants.DefaultHttpPort;
            }
            else
            {
                this.DestinationsApiUrl = destinationsApiAddress;
                this.Port = port;
            }

            if (messageApiUrl.Contains("http://localhost:") && string.IsNullOrEmpty(messageApiPort))
            {
                var uri = new Uri(messageApiUrl);
                this.MessageApiPort = uri.GetComponents(UriComponents.Port, UriFormat.Unescaped);
                this.MessageApiBaseUrl = uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped).TrimEnd('/');
            }
            else if (destinationsApiAddress.Equals("http://localhost") && string.IsNullOrEmpty(port))
            {
                this.MessageApiBaseUrl = messageApiUrl;
                this.MessageApiPort = AdhocConstants.DefaultHttpPort;
            }
            else
            {
                this.MessageApiBaseUrl = messageApiUrl;
                this.MessageApiPort = messageApiPort;
            }

            this.HttpClientTimeoutInMilliseconds = httpTimeOut;
            this.TenentId = tenentId;
        }

        public string DestinationsApiUrl { get; set; }
        public int HttpClientTimeoutInMilliseconds { get; set; }
        public string MessageApiBaseUrl { get; set; }
        public string MessageApiPort { get; set; }
        public string Port { get; }
        public string TenentId { get; set; }

    }
}