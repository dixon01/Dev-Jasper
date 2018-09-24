// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpServer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.IO;

    using NLog;

    /// <summary>
    /// A simple HTTP server.
    /// </summary>
    public abstract class HttpServer : IDisposable
    {
        private const string NewLine = "\r\n";

        private static readonly Logger Logger = LogHelper.GetLogger<HttpServer>();

        private readonly TcpListener listener;

        private Thread acceptThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServer"/> class.
        /// </summary>
        /// <param name="endPoint">
        /// The local end point to bind the server to.
        /// </param>
        protected HttpServer(IPEndPoint endPoint)
        {
            this.listener = new TcpListener(endPoint);
        }

        /// <summary>
        /// Gets the local IP addresses on which this server is listening.
        /// </summary>
        public IPAddress[] LocalAddresses
        {
            get
            {
                var endpoint = (IPEndPoint)this.listener.LocalEndpoint;
                if (!endpoint.Address.Equals(IPAddress.Any))
                {
                    return new[] { endpoint.Address };
                }

                var addresses = new List<IPAddress>();
                foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        addresses.Add(ip);
                    }
                }

                return addresses.ToArray();
            }
        }

        /// <summary>
        /// Gets the local port on which this server is listening.
        /// This property is only valid after calling <see cref="Start"/>.
        /// </summary>
        public int LocalPort
        {
            get
            {
                return ((IPEndPoint)this.listener.LocalEndpoint).Port;
            }
        }

        /// <summary>
        /// Starts this server.
        /// </summary>
        public void Start()
        {
            if (this.acceptThread != null)
            {
                return;
            }

            this.listener.Start();
            Logger.Debug("Listener started on {0}", this.listener.LocalEndpoint);

            this.acceptThread = new Thread(this.RunAccept) { Name = "HttpServer.Accept", IsBackground = true };
            this.acceptThread.Start();
        }

        /// <summary>
        /// Stops this server.
        /// </summary>
        public void Stop()
        {
            if (this.acceptThread == null)
            {
                return;
            }

            this.acceptThread = null;
            this.listener.Stop();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// Method to be implemented by subclasses to handle a new HTTP request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        protected abstract void HandleRequest(Request request);

        private void RunAccept()
        {
            while (this.acceptThread != null)
            {
                try
                {
                    var client = this.listener.AcceptTcpClient();
                    var processor = new HttpProcessor(client, this);
                    processor.Start();
                }
                catch (ObjectDisposedException)
                {
                    // ignore, this means we were closed
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't accept TCP client");
                }
            }
        }

        /// <summary>
        /// The HTTP request.
        /// </summary>
        public class Request
        {
            private readonly Stream input;

            private readonly Stream output;

            /// <summary>
            /// Initializes a new instance of the <see cref="Request"/> class.
            /// </summary>
            /// <param name="method">
            /// The method.
            /// </param>
            /// <param name="path">
            /// The path.
            /// </param>
            /// <param name="input">
            /// The input.
            /// </param>
            /// <param name="output">
            /// The output.
            /// </param>
            public Request(string method, string path, Stream input, Stream output)
            {
                this.input = input;
                this.output = output;

                this.Method = method;
                this.Path = path;
            }

            /// <summary>
            /// Gets the method (GET or POST).
            /// </summary>
            public string Method { get; private set; }

            /// <summary>
            /// Gets the path of the request including the query string.
            /// </summary>
            public string Path { get; private set; }

            /// <summary>
            /// Gets the response.
            /// </summary>
            /// <returns>
            /// The <see cref="Response"/>.
            /// </returns>
            public Response GetResponse()
            {
                return new Response(this.output);
            }

            /// <summary>
            /// Gets the request stream.
            /// </summary>
            /// <returns>
            /// The <see cref="Stream"/>.
            /// </returns>
            public Stream GetRequestStream()
            {
                return this.input;
            }
        }

        /// <summary>
        /// The response.
        /// </summary>
        public class Response
        {
            private readonly Stream output;

            private bool headerWritten;

            /// <summary>
            /// Initializes a new instance of the <see cref="Response"/> class.
            /// </summary>
            /// <param name="output">
            /// The output.
            /// </param>
            public Response(Stream output)
            {
                this.output = output;
                this.ContentType = "text/html";
            }

            /// <summary>
            /// Gets or sets the content type.
            /// Default value is text/html.
            /// </summary>
            public string ContentType { get; set; }

            /// <summary>
            /// The get response stream.
            /// </summary>
            /// <returns>
            /// The <see cref="Stream"/>.
            /// </returns>
            public Stream GetResponseStream()
            {
                if (this.headerWritten)
                {
                    return this.output;
                }

                this.headerWritten = true;
                var writer = new StreamWriter(this.output);
                writer.NewLine = NewLine;
                writer.WriteLine("HTTP/1.0 200 OK");
                writer.WriteLine("Content-Type: " + this.ContentType);
                writer.WriteLine("Connection: close");
                writer.WriteLine();
                writer.Flush();

                return this.output;
            }
        }

        private class HttpProcessor
        {
            private readonly TcpClient client;

            private readonly HttpServer server;

            private readonly Dictionary<string, string> httpHeaders = new Dictionary<string, string>();

            private Stream inputStream;

            private Stream outputStream;

            private string httpMethod;

            private string httpPath;

            public HttpProcessor(TcpClient s, HttpServer server)
            {
                this.client = s;
                this.server = server;
            }

            public void Start()
            {
                ThreadPool.QueueUserWorkItem(s => this.Process());
            }

            private void Process()
            {
                Logger.Trace("Handling request from {0}", this.client.Client.RemoteEndPoint);

                // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
                // "processed" view of the world, and we want the data raw after the headers
                this.inputStream = new BufferedStream(this.client.GetStream());

                // we probably shouldn't be using a streamwriter for all output from handlers either
                this.outputStream = new BufferedStream(this.client.GetStream());
                try
                {
                    this.ParseRequest();
                    this.ReadHeaders();
                    if (this.httpMethod.Equals("GET"))
                    {
                        this.server.HandleRequest(
                            new Request(this.httpMethod, this.httpPath, Stream.Null, this.outputStream));
                    }
                    else if (this.httpMethod.Equals("POST"))
                    {
                        this.HandlePostRequest();
                    }
                }
                catch (Exception ex)
                {
                    this.WriteFailure(ex);
                }

                this.inputStream = null;
                this.outputStream = null;
                this.client.Close();
            }

            private void ParseRequest()
            {
                var request = this.ReadRequestLine();
                string[] tokens = request.Split(' ');
                if (tokens.Length != 3)
                {
                    throw new Exception("invalid http request line");
                }

                this.httpMethod = tokens[0].ToUpper();
                this.httpPath = tokens[1];
                ////httpProtocolVersion = tokens[2];

                Logger.Trace("HTTP request: {0}", request);
            }

            private void ReadHeaders()
            {
                string line;
                while ((line = this.ReadRequestLine()) != null)
                {
                    if (line.Equals(string.Empty))
                    {
                        // got all headers
                        return;
                    }

                    int separator = line.IndexOf(':');
                    if (separator == -1)
                    {
                        throw new Exception("invalid http header line: " + line);
                    }

                    string name = line.Substring(0, separator);
                    int pos = separator + 1;
                    while ((pos < line.Length) && (line[pos] == ' '))
                    {
                        pos++; // strip any spaces
                    }

                    string value = line.Substring(pos, line.Length - pos);
                    this.httpHeaders[name] = value;
                }
            }

            private string ReadRequestLine()
            {
                var data = new StringBuilder();
                while (true)
                {
                    int c = this.inputStream.ReadByte();
                    if (c == '\n')
                    {
                        break;
                    }

                    if (c == '\r')
                    {
                        continue;
                    }

                    if (c == -1)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    data.Append((char)c);
                }

                return data.ToString();
            }

            private void HandlePostRequest()
            {
                var input = this.inputStream;
                if (this.httpHeaders.ContainsKey("Content-Length"))
                {
                    input = new ContentStream(this.inputStream, Convert.ToInt32(this.httpHeaders["Content-Length"]));
                }

                this.server.HandleRequest(new Request(this.httpMethod, this.httpPath, input, this.outputStream));
            }

            private void WriteFailure(Exception exception)
            {
                try
                {
                    int errorCode = 500;
                    var shortErrorMessage = "Server Error";
                    var longErrorMessage = exception.GetType().Name + ": " + exception.Message;

                    if (exception is FileNotFoundException)
                    {
                        errorCode = 404;
                        shortErrorMessage = "File Not Found";
                        longErrorMessage = exception.Message;
                    }

                    Logger.Info(exception, "Request handling caused an exception, returning error page errorCode = {0}", errorCode);

                    var writer = new StreamWriter(this.outputStream);
                    writer.NewLine = NewLine;
                    writer.WriteLine("HTTP/1.0 {0} {1}", errorCode, shortErrorMessage);
                    writer.WriteLine("Connection: close");
                    writer.WriteLine();
                    writer.WriteLine(
                        "<html><head><title>{0} {1}</title></head><body><h1>{0} {1}</h1><p>{2}</p></body></html>",
                        errorCode,
                        shortErrorMessage,
                        longErrorMessage);

                    writer.Flush();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't send error page");
                }
            }
        }
    }
}