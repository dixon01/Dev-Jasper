namespace GenericViewTestUI
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transcoder.Xml;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Protocols.Ximple;

    public partial class Form1 : Form
    {
        private bool mediInitialized;

        public Form1()
        {
            this.InitializeComponent();

            this.form1BindingSource.DataSource = this;
        }

        private void XimpleReceived(object sender, MessageEventArgs<Ximple> e)
        {
            this.Invoke(new Action<Ximple>(this.tabControl1.AddXimple), e.Message);
        }

        private void BtnConfigureClick(object sender, EventArgs e)
        {
            // configure message dispatcher to have an EventHandler port
            var config = new MediConfig();
            var eh = new EventHandlerPeerConfig
            {
                SupportedMessages = { typeof(Ximple).FullName },
                LocalPort = (int)this.numericUpDown1.Value
            };
            config.Peers.Add(eh);
            var medi = new ServerPeerConfig
            {
                Codec = new XmlCodecConfig(),
                Transport = new TcpTransportServerConfig { LocalPort = (int)this.numericUpDown2.Value }
            };
            config.Peers.Add(medi);
            var configurator = new ObjectConfigurator(config);
            MessageDispatcher.Instance.Configure(configurator);

            if (!this.mediInitialized)
            {
                MessageDispatcher.Instance.Subscribe<Ximple>(this.XimpleReceived);
                this.mediInitialized = true;
            }
        }
    }
}
