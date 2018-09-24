// -----------------------------------------------------------------------
// <copyright file="TelnetClient.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AnnaxRendererTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    using NLog;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TelnetClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private TcpClient client;

        private StreamWriter writer;

        public TelnetClient()
        {
        }

        public event EventHandler Connected;

        public void Start()
        {
            if (this.client != null)
            {
                return;
            }

            this.client = new TcpClient();
            Logger.Info("Connecting to display");
            this.client.BeginConnect("192.168.192.200", 23, this.OnConnected, null);
        }

        public void Stop()
        {
            if (this.client == null)
            {
                return;
            }

            Logger.Info("Closing connection");
            this.client.Close();
            this.client = null;

            if (this.writer != null)
            {
                this.writer.Dispose();
                this.writer = null;
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            var w = this.writer;
            if (w == null)
            {
                return;
            }

            var line = string.Format(format, args);

            Logger.Trace("Writing '{0}'", line);
            w.WriteLine(line);
        }

        private void OnConnected(IAsyncResult ar)
        {
            this.client.EndConnect(ar);
            Logger.Info("Connected to display");
            var stream = this.client.GetStream();
            this.writer = new StreamWriter(stream, Encoding.ASCII);

            var reader = new Thread(this.ReadThread) { IsBackground = true };
            reader.Start(stream);

            Thread.Sleep(1000);
            // write the password
            this.WriteLine("prb");
            this.RaiseConnected(EventArgs.Empty);
        }

        private void ReadThread(object state)
        {
            var stream = (NetworkStream)state;
            var reader = new StreamReader(stream, Encoding.ASCII);
            int read;
            char[] buffer = new char[10];
            while (this.client != null && (read = reader.ReadBlock(buffer, 0, buffer.Length)) > 0)
            {
                Logger.Trace("Read: '{0}'", new string(buffer, 0, read));
            }

            reader.Dispose();
        }

        private void RaiseConnected(EventArgs e)
        {
            var handler = this.Connected;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
