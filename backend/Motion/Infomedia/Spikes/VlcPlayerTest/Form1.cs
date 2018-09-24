using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VlcPlayerTest
{
    using Vlc.DotNet.Core;
    using Vlc.DotNet.Core.Medias;
    using Vlc.DotNet.Forms;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonPlayClick(object sender, EventArgs e)
        {
            if (this.buttonPlay.Text == "Play")
            {
                if (this.textBoxUrl.Text.Contains("://"))
                {
                    this.vlcControl1.Media = new LocationMedia(this.textBoxUrl.Text);
                }
                else
                {
                    this.vlcControl1.Media = new PathMedia(this.textBoxUrl.Text);
                }

                this.vlcControl1.Play();
                this.buttonPlay.Text = "Pause";
            }
            else if (this.buttonPlay.Text == "Resume")
            {
                this.vlcControl1.Play();
            }
            else
            {
                this.vlcControl1.Pause();
                this.buttonPlay.Text = "Resume";
            }
        }

        private void ButtonBrowseClick(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.textBoxUrl.Text = this.openFileDialog.FileName;
            this.buttonPlay.Text = "Play";
        }

        private void VlcControl1Stopped(VlcControl sender, VlcEventArgs<EventArgs> e)
        {
        }

        private void VlcControl1EndReached(VlcControl sender, VlcEventArgs<EventArgs> e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new VlcEventHandler<VlcControl, EventArgs>(this.VlcControl1EndReached), sender, e);
                return;
            }

            this.buttonPlay.Text = "Play";
        }
    }
}
