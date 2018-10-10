
namespace TcpServerSocketTest
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal class TcpServer
    {
        static void Main(string[] args)
        {
            var server = new TcpServer();
            var port = args.Length == 0 ? 1596 : int.Parse(args[0]);
            Console.WriteLine("Binding to TCP Socket port " + port);
            var socket = server.BindSocket(port);
            socket.Close();
            Console.WriteLine("Socket Closed");
        }

        public Socket BindSocket(int localPort)
        {
            try
            {
                var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(IPAddress.Any, localPort);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(10);
                return serverSocket;
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
