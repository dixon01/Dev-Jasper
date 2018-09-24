namespace ARSoft.Tools.Net
{

#if WindowsCE
    using System;
    using System.Net;
    using System.Net.Sockets;

    using OpenNETCF.Net.NetworkInformation;
#else
    using System.Net.NetworkInformation;
#endif

    internal static class Extensions
    {
        public static bool Contains(this string value, char c)
        {
            return value.IndexOf(c) >= 0;
        }

#if WindowsCE
        public static bool SupportsMulticast(this INetworkInterface n)
        {
            return true;
        }
#else
        public static bool SupportsMulticast(this NetworkInterface n)
        {
            return n.SupportsMulticast;
        }
#endif

#if WindowsCE
        public static bool SupportsIPv4(this INetworkInterface n)
        {
            return true;
        }
#else
        public static bool SupportsIPv4(this NetworkInterface n)
        {
            return (n.Supports(NetworkInterfaceComponent.IPv4));
        }
#endif

        public static bool TryParseInt(this string s, out int result)
        {
#if WindowsCE
            try
            {
                result = int.Parse(s);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
#else
            return int.TryParse(s, out result);
#endif
        }

#if WindowsCE

        public static IAsyncResult BeginConnect(this TcpClient client, IPAddress address, int port, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public static void EndConnect(this TcpClient client, IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public static IAsyncResult BeginAcceptTcpClient(this TcpListener listener, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public static TcpClient EndAcceptTcpClient(this TcpListener listener, IAsyncResult result)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
