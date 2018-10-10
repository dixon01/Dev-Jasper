// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonServer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JsonRpcServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JsonRpcServer
{
    using System.Windows.Forms;

    using Gorba.Motion.Protran.IntegrationTests.Json;

    /// <summary>
    /// The json rpc server.
    /// </summary>
    public partial class JsonServer : Form
    {
        private JsonRpcServer jsonRpcServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonServer"/> class.
        /// </summary>
        public JsonServer()
        {
            this.InitializeComponent();
        }

        private void ButtonStartClick(object sender, System.EventArgs e)
        {
            this.jsonRpcServer = new JsonRpcServer(3011);
            this.jsonRpcServer.Start();
        }

        private void ButtonStopClick(object sender, System.EventArgs e)
        {
            this.jsonRpcServer.Dispose();
        }

        private void ButtonSendTelegramClick(object sender, System.EventArgs e)
        {
            var telegram = this.jsonRpcServer.FromHexToAscii(this.textBoxTelegram.Text);
            this.jsonRpcServer.SendTelegram(telegram);
        }
    }
}
