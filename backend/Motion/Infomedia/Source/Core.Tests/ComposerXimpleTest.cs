// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="ComposerXimpleTest.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Core.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Transport.Stream;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>The composer ximple test.</summary>
    [TestClass]
    public class ComposerXimpleTest
    {
        #region Public Methods and Operators

        /// <summary>The get ip4 address.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetIP4Address()
        {
            string IP4Address = string.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }

        #endregion

        #region Methods

        private static void Send(TcpClient client, string data)
        {
            if (client.Connected)
            {
                var buffer = Encoding.UTF8.GetBytes(data);
                client.GetStream().Write(buffer, 0, buffer.Length);
            }
        }

        private string CreateHandshake(
            StreamFeature features = StreamFeature.MessagesType | StreamFeature.Framing, 
            int sessionId = 123, 
            string codecName = "BecCodec", 
            string version = "2.0.0")
        {
            var localAddress = new MediAddress(Environment.MachineName, "Composer");

            // send handshake version, transcoder identification, session ID and local address
            var id = string.Format(
                "<{0},{1}{2},{3},{4}:{5}>", 
                (byte)features, 
                codecName, 
                version, 
                sessionId, 
                localAddress.Unit.Replace('>', '_'), 
                localAddress.Application.Replace('>', '_'));
            return id;
        }

        private void WriteHandshake(
            Stream stream, 
            StreamFeature features = StreamFeature.MessagesType | StreamFeature.Framing, 
            string codecName = "", 
            string version = "2.0.0")
        {
            // int sessionId = this.agreedSessionId == null ? this.initialSessionId : this.agreedSessionId.Id;
            int sessionId = 123;
            var localAddress = new MediAddress(Environment.MachineName, "Composer");

            // send handshake version, transcoder identification, session ID and local address
            var id = string.Format(
                "<{0},{1}{2},{3},{4}:{5}>", 
                (byte)features, 
                codecName, 
                version, 
                sessionId, 
                localAddress.Unit.Replace('>', '_'), 
                localAddress.Application.Replace('>', '_'));
            Debug.WriteLine("Writting HandShake");
            var idData = Encoding.UTF8.GetBytes(id);
            stream.Write(idData, 0, idData.Length);
        }

        #endregion
    }
}