namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI
{
    using Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp;

    using System;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        private readonly RtspServer server = new RtspServer();

        public Form1()
        {
            this.InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.server.Start(554);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var packet in new JpegSource().GetPackets())
            {
                Console.WriteLine(packet);
            }
        }
    }
}
