namespace Luminator.AdhocMessaging
{
    using System;

    public class AdhocConfiguration
    {
        public const string DefaultTenentId = "7141d8e5-b8e1-425a-21e5-08d541a9fc53";

        public AdhocConfiguration(string baseAddress = "http://localhost", string port = "63093", int pollInterval = 60000, string tenentId = DefaultTenentId)
        {
            if (baseAddress.Contains("http://localhost:") && string.IsNullOrEmpty(port))
            {
                var uri = new Uri(baseAddress);
                this.Port = uri.GetComponents(UriComponents.Port, UriFormat.Unescaped);
                this.BaseUrl = uri.GetComponents( UriComponents.AbsoluteUri & ~UriComponents.Port,UriFormat.UriEscaped).TrimEnd('/');
            }
            else if (baseAddress.Equals("http://localhost") && string.IsNullOrEmpty(port))
            {
                this.BaseUrl = baseAddress;
                this.Port = "63093";
            }
            else
            {
                this.BaseUrl = baseAddress;
                this.Port = port;
            }
           
            this.PollInterval = pollInterval;
            this.TenentId = tenentId;
        }

        public string BaseUrl { get; set; }

        public string Port { get; }

        public string TenentId { get; set; }

        public int PollInterval { get; }
    }
}