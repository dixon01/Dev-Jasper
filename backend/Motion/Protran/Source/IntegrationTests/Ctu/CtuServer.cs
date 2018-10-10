// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtuServer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Ctu
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using Gorba.Common.Protocols.Ctu;
    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Common.Protocols.Ctu.Notifications;
    using Gorba.Common.Protocols.Ctu.Responses;
    using Gorba.Motion.Protran.Core.Buffers;

    /// <summary>
    /// Object tasked to act as the CU5 device
    /// just only for the activities related to the CTU protocol.
    /// </summary>
    public class CtuServer
    {
        private readonly string addressInWhichListen;
        private readonly int portInWhichListen;
        private readonly string protranIp;

        /// <summary>
        /// Object tasked to serialize/deserialize CTU datagrams in
        /// buffers and vice versa, respectively.
        /// </summary>
        private readonly CtuSerializer ctuSerializer;

        private ushort seqNum;
        private Socket socket;
        private byte[] socketBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CtuServer"/> class.
        /// </summary>
        /// <param name="ip">The ip in which will listen the CTU server.</param>
        /// <param name="port">The port in which will listen the CTU server.</param>
        /// <param name="protranIp">The IP address of Protran.</param>
        public CtuServer(string ip, int port, string protranIp)
        {
            this.protranIp = protranIp;
            this.portInWhichListen = port;
            this.addressInWhichListen = ip;
            this.ctuSerializer = new CtuSerializer();
            this.TripletsExpectations = new List<Triplet>();
        }

        /// <summary>
        /// Gets the current amount of errors.
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Gets the list of all the CTU triplet that this CTU expects to receive from Protran.
        /// </summary>
        public List<Triplet> TripletsExpectations { get; private set; }

        /// <summary>
        /// Gets the total amount of expected triplets received so far by this CTU server.
        /// </summary>
        public int ExpectedTripletsReceived { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this CTU server
        /// is currently connected with Protran or not.
        /// </summary>
        public bool IsConnectedWithProtran { get; private set; }

        /// <summary>
        /// Starts the CTU server.
        /// </summary>
        public void Start()
        {
            // the user wants to start
            IPAddress ip;
            bool ok = IPAddress.TryParse(this.addressInWhichListen, out ip);
            if (!ok)
            {
                // invalid IP.
                return;
            }

            var localEndPoint = new IPEndPoint(IPAddress.Parse(this.addressInWhichListen), this.portInWhichListen);
            Console.WriteLine("Binding to local endpoint: {0}", localEndPoint);

            this.socketBuffer = new byte[4096];
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.socket.Bind(localEndPoint);
            this.socket.BeginReceive(
                this.socketBuffer, 0, this.socketBuffer.Length, SocketFlags.None, this.ReceiveCallback, null);
        }

        /// <summary>
        /// Stops the CTU server.
        /// </summary>
        public void Stop()
        {
            // the user wants to stop
            this.socket.Close();
            this.socket = null;
        }

        /// <summary>
        /// Sets an expectation to this CTU server.
        /// Should be invoked immediately before sending an ISI get or ISI put to Protran.
        /// </summary>
        /// <param name="expectedTriplet">The triplet to expect.</param>
        public void ExpectTriplet(Triplet expectedTriplet)
        {
            this.TripletsExpectations.Add(expectedTriplet);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int bytesRead;
            try
            {
                bytesRead = this.socket.EndReceive(ar);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error on reading from the UDP socket: {0}", exc.Message);
                return;
            }

            var receivedBuffer = new byte[bytesRead];
            Array.Copy(this.socketBuffer, 0, receivedBuffer, 0, bytesRead);
            this.ManageReceivedBuffer(receivedBuffer);

            // let's re-enable the read for the next CTU datagram...
            this.socket.BeginReceive(
                this.socketBuffer, 0, this.socketBuffer.Length, SocketFlags.None, this.ReceiveCallback, null);
        }

        private void ManageReceivedBuffer(byte[] receivedBytes)
        {
            var datagram = this.ctuSerializer.Deserialize(receivedBytes);
            if (datagram == null)
            {
                // invalid datagram.
                this.ErrorCount++;
                Console.WriteLine("Error. Received a not serializable CTU triplet.");
                return;
            }

            var formatter = new StringBuilder();
            formatter.AppendLine(
                string.Format(
                    "Version: {0}\nFlags: {1}\n Seq. Num: {2}",
                    datagram.Header.VersionNumber,
                    datagram.Header.Flags,
                    datagram.Header.SequenceNumber));

            int tripletCounter = 0;
            foreach (var triplet in datagram.Payload.Triplets)
            {
                formatter.AppendLine(string.Format("Triplet {0}", tripletCounter++));
                formatter.AppendLine("\t Tag: " + triplet.Tag);
                formatter.AppendLine("\t Length: " + triplet.Length);
                formatter.AppendLine("\t Value: " + triplet);
                Console.WriteLine("Received triplet:\n{0}", formatter);
                switch (triplet.Tag)
                {
                    case TagName.DeviceInfoRequest:
                        this.SendDatagram(
                            new DeviceInfoResponse
                            {
                                SerialNumber = 123456,
                                SoftwareVersion = this.GetType().Assembly.GetName().Version.ToString(4),
                                DataVersion = "Integration Test CTU Server"
                            });

                        // the triplet "DeviceInfoRequest" was managed.
                        // I don't have to check it in the expectations,
                        // so, here below I continue with the next triplet in the list.
                        continue;

                    case TagName.Status:
                    case TagName.DeviceInfoResponse:
                        {
                            Console.WriteLine("Received periodic Protran triplet: {0}", triplet.Tag);
                        }

                        continue;

                    case TagName.DisplayStatus:
                    case TagName.LogMessage:
                    case TagName.DownloadStart:
                    case TagName.DownloadAbort:
                    case TagName.LineInfo:
                    case TagName.DownloadProgressRequest:
                    case TagName.DownloadProgressResponse:
                        {
                            // for this integration test,
                            // Protran shouldn't send to me none of those triplet.
                            Console.WriteLine("Protran shouldn't send this kind of triplet: {0}", triplet.Tag);
                            this.ErrorCount++;
                        }

                        continue;
                }

                // now I check if the current Triplet is inside the list
                // of the expected triplets
                this.CheckTriplet(triplet);
            }
        }

        private void CheckTriplet(Triplet triplet)
        {
            var extLineInfo = triplet as ExtendedLineInfo;
            if (extLineInfo != null)
            {
                Console.WriteLine("Received a triplet: {0}", extLineInfo.GetType().Name);
                this.ManageExtendedLineInfo(extLineInfo);
                return;
            }

            var tripInfo = triplet as TripInfo;
            if (tripInfo != null)
            {
                Console.WriteLine("Received a triplet: {0}", tripInfo.GetType().Name);
                this.ManageTripInfo(tripInfo);
                return;
            }

            var countdownNumber = triplet as CountdownNumber;
            if (countdownNumber != null)
            {
                Console.WriteLine("Received a triplet: {0}", countdownNumber.GetType().Name);
                this.ManageCountDownNumber(countdownNumber);
                return;
            }

            var specialInputInfo = triplet as SpecialInputInfo;
            if (specialInputInfo != null)
            {
                Console.WriteLine("Received a triplet: {0}", specialInputInfo.GetType().Name);
                this.ManageSpecialInputInfo(specialInputInfo);
                return;
            }

            this.ErrorCount++;
            Console.WriteLine("Received a not supported triplet.");
        }

        private void ManageSpecialInputInfo(SpecialInputInfo specialInputInfo)
        {
            lock (this.TripletsExpectations)
            {
                Triplet remove = null;
                foreach (var tripletsExpectation in this.TripletsExpectations)
                {
                    var expectedTriple = tripletsExpectation as SpecialInputInfo;
                    if (expectedTriple == null)
                    {
                        // this is not an expected triplet that I can check
                        // with the incoming one.
                        continue;
                    }

                    if (expectedTriple.SpecialInputState == specialInputInfo.SpecialInputState)
                    {
                        // found
                        remove = expectedTriple;
                        break;
                    }
                }

                if (remove == null)
                {
                    // this CTU server has received a triplet
                    // that is NOT in the list of the expected triplets.
                    return;
                }

                // this CTU server has received a triplet
                // that is in the list of the expected triplets.
                // I've to remove it.
                this.TripletsExpectations.Remove(remove);
                ++this.ExpectedTripletsReceived;
            }
        }

        private void ManageCountDownNumber(CountdownNumber countdownNumber)
        {
            lock (this.TripletsExpectations)
            {
                Triplet remove = null;
                foreach (var tripletsExpectation in this.TripletsExpectations)
                {
                    var expectedTriple = tripletsExpectation as CountdownNumber;
                    if (expectedTriple == null)
                    {
                        // this is not an expected triplet that I can check
                        // with the incoming one.
                        continue;
                    }

                    if (expectedTriple.Number == countdownNumber.Number)
                    {
                        // found
                        remove = expectedTriple;
                        break;
                    }
                }

                if (remove == null)
                {
                    // this CTU server has received a triplet
                    // that is NOT in the list of the expected triplets.
                    return;
                }

                // this CTU server has received a triplet
                // that is in the list of the expected triplets.
                // I've to remove it.
                this.TripletsExpectations.Remove(remove);
                ++this.ExpectedTripletsReceived;
            }
        }

        private void ManageExtendedLineInfo(ExtendedLineInfo extLineInfo)
        {
            lock (this.TripletsExpectations)
            {
                Triplet remove = null;
                foreach (var tripletsExpectation in this.TripletsExpectations)
                {
                    var expectedTriple = tripletsExpectation as ExtendedLineInfo;
                    if (expectedTriple == null)
                    {
                        // this is not an expected triplet that I can check
                        // with the incoming one.
                        continue;
                    }

                    byte[] currDirBuffer = BufferUtils.FromHexStringToByteArray(expectedTriple.CurrentDirectionNo);
                    byte[] destinationBuffer = BufferUtils.FromHexStringToByteArray(expectedTriple.Destination);
                    byte[] destArabicBuffer = BufferUtils.FromHexStringToByteArray(expectedTriple.DestinationArabic);
                    byte[] destNumberBuffer = BufferUtils.FromHexStringToByteArray(expectedTriple.DestinationNo);
                    byte[] lineNumberBuffer = BufferUtils.FromHexStringToByteArray(expectedTriple.LineNumber);
                    string clearTextCurrDirection = currDirBuffer != null
                                                        ? Encoding.UTF8.GetString(currDirBuffer)
                                                        : string.Empty;
                    string clearTextDestination = destinationBuffer != null
                                                      ? Encoding.UTF8.GetString(destinationBuffer)
                                                      : string.Empty;
                    string clearTextDestinationArab = destArabicBuffer != null
                                                          ? Encoding.UTF8.GetString(destArabicBuffer)
                                                          : string.Empty;
                    string clearTextDestNumber = destNumberBuffer != null
                                                     ? Encoding.UTF8.GetString(destNumberBuffer)
                                                     : string.Empty;
                    string clearTextLineNumber = lineNumberBuffer != null
                                                     ? Encoding.UTF8.GetString(lineNumberBuffer)
                                                     : string.Empty;
                    if (clearTextCurrDirection == extLineInfo.CurrentDirectionNo
                        && clearTextDestination == extLineInfo.Destination
                        && clearTextDestinationArab == extLineInfo.DestinationArabic
                        && clearTextDestNumber == extLineInfo.DestinationNo
                        && clearTextLineNumber == extLineInfo.LineNumber)
                    {
                        // found
                        remove = expectedTriple;
                        break;
                    }
                }

                if (remove == null)
                {
                    // this CTU server has received a triplet
                    // that is NOT in the list of the expected triplets.
                    return;
                }

                // this CTU server has received a triplet
                // that is in the list of the expected triplets.
                // I've to remove it.
                this.TripletsExpectations.Remove(remove);
                ++this.ExpectedTripletsReceived;
            }
        }

        private void ManageTripInfo(TripInfo tripInfo)
        {
            lock (this.TripletsExpectations)
            {
                Triplet remove = null;
                foreach (var tripletsExpectation in this.TripletsExpectations)
                {
                    var expectedTriple = tripletsExpectation as TripInfo;
                    if (expectedTriple == null)
                    {
                        // this is not an expected triplet that I can check
                        // with the incoming one.
                        continue;
                    }

                    byte[] destinationBuffer = BufferUtils.FromHexStringToByteArray(expectedTriple.Destination);
                    byte[] destArabicBuffer = BufferUtils.FromHexStringToByteArray(expectedTriple.DestinationArabic);
                    byte[] lineNumberBuffer = BufferUtils.FromHexStringToByteArray(expectedTriple.LineNumber);
                    string clearTextDestination = destinationBuffer != null
                                                      ? Encoding.UTF8.GetString(destinationBuffer)
                                                      : string.Empty;
                    string clearTextDestinationArab = destArabicBuffer != null
                                                          ? Encoding.UTF8.GetString(destArabicBuffer)
                                                          : string.Empty;
                    string clearTextLineNumber = lineNumberBuffer != null
                                                     ? Encoding.UTF8.GetString(lineNumberBuffer)
                                                     : string.Empty;
                    if (clearTextDestination == tripInfo.Destination
                        && clearTextDestinationArab == tripInfo.DestinationArabic
                        && clearTextLineNumber == tripInfo.LineNumber)
                    {
                        // found
                        remove = expectedTriple;
                        break;
                    }
                }

                if (remove == null)
                {
                    // this CTU server has received a triplet
                    // that is NOT in the list of the expected triplets.
                    return;
                }

                // this CTU server has received a triplet
                // that is in the list of the expected triplets.
                // I've to remove it.
                this.TripletsExpectations.Remove(remove);
                ++this.ExpectedTripletsReceived;
            }
        }

        private void SendDatagram(params Triplet[] triplets)
        {
            if (this.socket == null)
            {
                // invalid socket.
                return;
            }

            lock (this.socket)
            {
                var header = new Header { SequenceNumber = this.seqNum++ };
                var payload = new Payload { Triplets = new List<Triplet>(triplets) };

                var datagram = new CtuDatagram(header, payload);
                byte[] buffer = this.ctuSerializer.Serialize(datagram);

                try
                {
                    int bytesWrote = this.socket.SendTo(
                        buffer,
                        0,
                        buffer.Length,
                        SocketFlags.None,
                        new IPEndPoint(IPAddress.Parse(this.protranIp), this.portInWhichListen));
                    string format = (bytesWrote == buffer.Length)
                                        ? "Sent CTU datagram: {0}"
                                        : "Error on sending CTU datagram: {0}";
                    this.IsConnectedWithProtran = bytesWrote == buffer.Length;
                    Console.WriteLine(format, datagram);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Error on reading from the UDP socket: {0}", exc.Message);
                }
            }
        }
    }
}
