using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsMediaPlayerTest
{
    using Vlc.DotNet.Core;
    using Vlc.DotNet.Core.Medias;
    using Vlc.DotNet.Forms;

    public partial class VideoLanDotNetForm : Form
    {
        public VideoLanDotNetForm()
        {
            InitializeComponent();
        }

        private void Button1Click(object sender, EventArgs e)
        {
            this.vlcControl1.Media = new PathMedia(WindowsMediaPlayerGame.VideoPath);
            this.vlcControl1.Play();
            this.vlcControl1.Playing += this.VlcControl1OnPlaying;
            this.vlcControl1.Stopped += this.VlcControl1OnStopped;
        }

        private void VlcControl1OnStopped(VlcControl sender, VlcEventArgs<EventArgs> vlcEventArgs)
        {
        }

        private void VlcControl1OnPlaying(VlcControl sender, VlcEventArgs<EventArgs> vlcEventArgs)
        {
            this.vlcControl1.VideoProperties.IsFullscreen = true;
        }
    }
}
