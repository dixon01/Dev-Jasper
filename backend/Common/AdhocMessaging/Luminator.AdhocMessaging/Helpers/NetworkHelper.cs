namespace Luminator.AdhocMessaging.Helpers
{
    using System;
    using System.Runtime.InteropServices;

    public static class NetworkHelper
    {
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool CheckNet()
        {
            return InternetGetConnectedState(out _, 0);
        }

        public static bool IsValidUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }
    }
}