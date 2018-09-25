// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamHandshake.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamHandshake type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;
    using System.IO;
    using System.Text;

    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Handshake implementation used by <see cref="StreamMessageTransport"/>s
    /// to figure out which codec (version) they should use and to reuse
    /// a session id when the transport is reconnected.
    /// </summary>
    internal class StreamHandshake
    {
        // TODO: add more features when implemented
        private const StreamFeature DefaultFeatures = StreamFeature.Framing;

        // TODO: add more features when implemented
        private const StreamFeature SupportedFeatures =
            StreamFeature.TypeMask | StreamFeature.Framing | StreamFeature.Gateway;

        private static readonly Logger Logger = LogHelper.GetLogger<StreamHandshake>();

        private readonly IMessageCodec codec;

        private readonly MediAddress localAddress;

        private readonly int initialSessionId;

        private bool shakingHands;

        private StreamFeature features;

        private StreamSessionId agreedSessionId;

        private CodecIdentification agreedCodec;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamHandshake"/> class.
        /// </summary>
        /// <param name="codec">
        /// The codec that is on top of the transport.
        /// </param>
        /// <param name="localAddress">
        /// The local Medi address.
        /// </param>
        /// <param name="initialSessionId">
        /// The initial session id.
        /// </param>
        public StreamHandshake(IMessageCodec codec, MediAddress localAddress, int initialSessionId)
        {
            this.codec = codec;
            this.localAddress = localAddress;
            this.initialSessionId = initialSessionId;
        }

        /// <summary>
        /// Delegate to verify if a session id is known.
        /// </summary>
        /// <param name="sessionId">
        /// The session id to check.
        /// </param>
        /// <returns>
        /// True if the session id is known.
        /// </returns>
        public delegate bool SessionIdVerficiation(StreamSessionId sessionId);

        /// <summary>
        /// Event that is fired when the handshake was successful.
        /// </summary>
        public event EventHandler<HandshakeSuccessEventArgs> Connected;

        /// <summary>
        /// Event that is fired if the handshake failed.
        /// </summary>
        public event EventHandler<HandshakeErrorEventArgs> Failed;

        /// <summary>
        /// Entry point for stream clients. This will connect the given
        /// client to the server and then start the
        /// handshake. At the end, either the <see cref="Connected"/>
        /// or the <see cref="Failed"/> event is fired.
        /// </summary>
        /// <param name="client">
        /// The client that will be used to connect to the server.
        /// </param>
        /// <param name="type">
        /// The type of the channel to be opened.
        /// </param>
        /// <param name="isGateway">
        /// Flag indicating if the connection is to be handled as a gateway connection.
        /// </param>
        public void BeginConnectToServer(IStreamClient client, ChannelType type, bool isGateway)
        {
            if (this.shakingHands)
            {
                throw new NotSupportedException("Can't connect to server twice");
            }

            switch (type)
            {
                case ChannelType.Message:
                    this.features = StreamFeature.MessagesType;
                    break;
                case ChannelType.Stream:
                    this.features = StreamFeature.StreamsType;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            if (isGateway)
            {
                this.features |= StreamFeature.Gateway;
            }

            this.features |= DefaultFeatures;

            this.shakingHands = true;
            try
            {
                client.BeginConnect(this.ConnectedToServer, client);
            }
            catch (Exception ex)
            {
                this.HandleException("Could not connect to server", ex, HandshakeError.NoConnection, client);
            }
        }

        /// <summary>
        /// Entry point for stream servers. This will use the given
        /// connection that is already connected to a client and will start the
        /// handshake. At the end, either the <see cref="Connected"/>
        /// or the <see cref="Failed"/> event is fired.
        /// </summary>
        /// <param name="clientConnection">
        /// The connection, usually one returned by <see cref="IStreamServer.EndAccept"/>.
        /// </param>
        /// <param name="sessionIdVerficiation">
        /// The callback to verify the session id. This will be called to
        /// see if a session id given by the client is known in the server.
        /// </param>
        public void BeginConnectToClient(
            IStreamConnection clientConnection, SessionIdVerficiation sessionIdVerficiation)
        {
            if (this.shakingHands)
            {
                throw new NotSupportedException("Can't connect to server twice");
            }

            this.shakingHands = true;
            try
            {
                var codecId = this.ReadHandshake(clientConnection.Stream);

                // verify if we support the sent session ID
                if (!sessionIdVerficiation(this.agreedSessionId))
                {
                    this.agreedSessionId = new StreamSessionId(this.initialSessionId, this.agreedSessionId.RemoteName);
                }

                // only call CheckCodec after our session ID is final,
                // otherwise some codecs could have problems matching codecs and sessions
                var supportedId = this.CheckCodec(codecId);
                if (supportedId == null)
                {
                    this.SendNackAndClose(clientConnection, HandshakeError.CodecNotSupported);
                    return;
                }

                this.agreedCodec = supportedId;
                this.WriteHandshake(clientConnection, supportedId, this.HandshakeWrittenToClient);
            }
            catch (HandshakeException ex)
            {
                this.SendNackAndClose(clientConnection, ex.Cause);
            }
            catch (Exception ex)
            {
                this.HandleException("Could not connect to client", ex, HandshakeError.StreamError, clientConnection);
            }
        }

        private static int ReadInteger(BinaryReader reader, out char nextChar)
        {
            var builder = new StringBuilder(16);
            char read;
            while (char.IsDigit(read = reader.ReadChar()) || (builder.Length == 0 && read == '-'))
            {
                builder.Append(read);
            }

            nextChar = read;

            return builder.Length == 0 ? 0 : int.Parse(builder.ToString());
        }

        private static string ReadString(BinaryReader reader, char delimiter)
        {
            var builder = new StringBuilder();
            char read;
            while ((read = reader.ReadChar()) != delimiter)
            {
                builder.Append(read);
            }

            return builder.ToString();
        }

        private void RaiseConnected(HandshakeSuccessEventArgs e)
        {
            if (!this.shakingHands)
            {
                return;
            }

            this.shakingHands = false;
            var handler = this.Connected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseFailed(HandshakeErrorEventArgs e)
        {
            if (!this.shakingHands)
            {
                return;
            }

            this.shakingHands = false;
            var handler = this.Failed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ConnectedToServer(IAsyncResult result)
        {
            IStreamClient client = null;
            try
            {
                client = result.AsyncState as IStreamClient;
                if (client == null)
                {
                    Logger.Error("ConnectedToServer with no IStreamClient as state");
                    this.RaiseFailed(new HandshakeErrorEventArgs(HandshakeError.InternalError));
                    return;
                }

                var conection = client.EndConnect(result);

                this.WriteHandshake(conection, this.codec.Identification, this.HandshakeWrittenToServer);
            }
            catch (Exception ex)
            {
                this.HandleException("Could not connect stream client", ex, HandshakeError.StreamError, client);
            }
        }

        private void WriteHandshake(IStreamConnection connection, CodecIdentification codecId, AsyncCallback callback)
        {
            int sessionId = this.agreedSessionId == null ? this.initialSessionId : this.agreedSessionId.Id;

            // send handshake version, transcoder identification, session ID and local address
            var id = string.Format(
                "<{0},{1}{2},{3},{4}:{5}>",
                (byte)this.features,
                codecId.Name,
                codecId.Version,
                sessionId,
                this.localAddress.Unit.Replace('>', '_'),
                this.localAddress.Application.Replace('>', '_'));
            Logger.Trace("Writing handshake: {0}", id);
            var idData = Encoding.UTF8.GetBytes(id);
            connection.Stream.BeginWrite(idData, 0, idData.Length, callback, connection);
        }

        private void HandshakeWrittenToServer(IAsyncResult result)
        {
            IStreamConnection connection = null;
            try
            {
                connection = result.AsyncState as IStreamConnection;
                if (connection == null)
                {
                    Logger.Error("HandshakeWrittenToServer with no NetworkStream as state");
                    this.RaiseFailed(new HandshakeErrorEventArgs(HandshakeError.InternalError));
                    return;
                }

                var stream = connection.Stream;
                stream.EndWrite(result);
                stream.Flush();

                var codecId = this.ReadHandshake(stream);
                var supportedId = this.CheckCodec(codecId);
                if (supportedId == null)
                {
                    this.SendNackAndClose(connection, HandshakeError.CodecNotSupported);
                    return;
                }

                Logger.Trace("Send Ack: <1>");
                var ack = Encoding.UTF8.GetBytes("<1>");
                stream.BeginWrite(ack, 0, ack.Length, this.AckWritten, connection);
            }
            catch (HandshakeException ex)
            {
                this.SendNackAndClose(connection, ex.Cause);
            }
            catch (Exception ex)
            {
                this.HandleException("Could not write handshake to server", ex, HandshakeError.StreamError, connection);
            }
        }

        private void HandshakeWrittenToClient(IAsyncResult result)
        {
            IStreamConnection connection = null;
            try
            {
                connection = result.AsyncState as IStreamConnection;
                if (connection == null)
                {
                    Logger.Error("HandshakeWrittenToClient with no IStreamConnection as state");
                    this.RaiseFailed(new HandshakeErrorEventArgs(HandshakeError.InternalError));
                    return;
                }

                var stream = connection.Stream;
                stream.EndWrite(result);
                stream.Flush();

                var error = this.ReadAck(stream);
                if (error != HandshakeError.Sucess)
                {
                    try
                    {
                        this.RaiseFailed(new HandshakeErrorEventArgs(error));
                    }
                    finally
                    {
                        connection.Dispose();
                    }
                }
                else
                {
                    this.RaiseConnected(
                        new HandshakeSuccessEventArgs(
                            connection, this.agreedSessionId, this.agreedCodec, this.features));
                }
            }
            catch (Exception ex)
            {
                this.HandleException("Could not write handshake to client", ex, HandshakeError.StreamError, connection);
            }
        }

        private HandshakeError ReadAck(Stream stream)
        {
            var reader = new BinaryReader(stream, Encoding.UTF8);
            char start = reader.ReadChar();
            if (start != '<')
            {
                Logger.Error("ReadAck: wrong start char: {0}", start);
                return HandshakeError.BadContent;
            }

            char end;
            var answer = ReadInteger(reader, out end);

            if (end != '>')
            {
                Logger.Error("ReadAck: wrong end char: {0}", end);
                return HandshakeError.BadContent;
            }

            Logger.Trace("Read ack: <{0}> ({1})", answer, (HandshakeError)answer);
            return (HandshakeError)answer;
        }

        private CodecIdentification ReadHandshake(Stream stream)
        {
            var reader = new BinaryReader(stream, Encoding.UTF8);
            char start = reader.ReadChar();
            if (start != '<')
            {
                Logger.Error("ReadHandshake: wrong start char: {0}", start);
                throw new HandshakeException(HandshakeError.BadContent);
            }

            char next;
            var featuresValue = ReadInteger(reader, out next);
            if (next == '>')
            {
                // we got a NACK instead of a handshake, let's fail with the reason
                throw new HandshakeException((HandshakeError)featuresValue);
            }

            if (next != ',')
            {
                Logger.Error("ReadHandshake: wrong delim char: {0}", next);
                throw new HandshakeException(HandshakeError.BadContent);
            }

            var channelType = (StreamFeature)featuresValue & StreamFeature.TypeMask;
            if (channelType != StreamFeature.MessagesType && channelType != StreamFeature.StreamsType)
            {
                Logger.Error("ReadHandshake: unsupported handshake type: {0:X2}", featuresValue);
                throw new HandshakeException(HandshakeError.VersionNotSupported);
            }

            this.features = (StreamFeature)featuresValue & SupportedFeatures;

            char name = reader.ReadChar();
            var version = ReadInteger(reader, out next);
            var codecId = new CodecIdentification(name, version);
            if (next != ',')
            {
                Logger.Error("ReadHandshake: wrong delim char: {0}", next);
                throw new HandshakeException(HandshakeError.BadContent);
            }

            var sessionId = ReadInteger(reader, out next);
            if (next != ',')
            {
                Logger.Error("ReadHandshake: wrong end char: {0}", next);
                throw new HandshakeException(HandshakeError.BadContent);
            }

            var remoteName = ReadString(reader, '>');
            this.agreedSessionId = new StreamSessionId(sessionId, remoteName);

            Logger.Trace("Read handshake: <{0},{1}{2},{3},{4}>", featuresValue, name, version, sessionId, remoteName);
            return codecId;
        }

        private CodecIdentification CheckCodec(CodecIdentification codecId)
        {
            var supportedId = this.codec.CheckSupport(this.agreedSessionId, codecId);
            if (supportedId == null)
            {
                Logger.Error("CheckCodec: unsupported transcoder: {0}", codecId);
                return null;
            }

            return supportedId;
        }

        private void AckWritten(IAsyncResult result)
        {
            IStreamConnection connection = null;
            try
            {
                connection = result.AsyncState as IStreamConnection;
                if (connection == null)
                {
                    Logger.Error("AckWritten with no IStreamConnection as state");
                    this.RaiseFailed(new HandshakeErrorEventArgs(HandshakeError.InternalError));
                    return;
                }

                this.RaiseConnected(
                    new HandshakeSuccessEventArgs(connection, this.agreedSessionId, this.agreedCodec, this.features));
            }
            catch (Exception ex)
            {
                this.HandleException("Could not write ACK", ex, HandshakeError.StreamError, connection);
            }
        }

        private void SendNackAndClose(IStreamConnection connection, HandshakeError cause)
        {
            try
            {
                this.RaiseFailed(new HandshakeErrorEventArgs(cause));

                var nack = string.Format("<{0}>", (int)cause);
                Logger.Trace("Writing nack: {0}", nack);
                var bytes = Encoding.UTF8.GetBytes(nack);
                connection.Stream.BeginWrite(bytes, 0, bytes.Length, this.NackWritten, connection);
            }
            catch (Exception ex)
            {
                this.HandleException("Could not send NACK", ex, HandshakeError.StreamError, connection);
            }
        }

        private void NackWritten(IAsyncResult result)
        {
            IStreamConnection connection = null;
            try
            {
                connection = result.AsyncState as IStreamConnection;
                if (connection == null)
                {
                    Logger.Error("NackWritten with no IStreamConnection as state");
                    return;
                }

                connection.Dispose();
            }
            catch (Exception ex)
            {
                this.HandleException("Could not send NACK", ex, HandshakeError.StreamError, connection);
            }
        }

        private void HandleException(string reason, Exception ex, HandshakeError cause, IDisposable resource)
        {
            Logger.Warn(ex, reason);
            this.RaiseFailed(new HandshakeErrorEventArgs(cause));
            if (resource != null)
            {
                resource.Dispose();
            }
        }

        private class HandshakeException : Exception
        {
            public HandshakeException(HandshakeError error)
            {
                this.Cause = error;
            }

            public HandshakeError Cause { get; private set; }
        }
    }
}
