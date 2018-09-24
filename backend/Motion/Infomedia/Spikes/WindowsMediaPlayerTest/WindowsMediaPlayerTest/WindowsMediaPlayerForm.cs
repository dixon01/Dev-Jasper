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
    using AxWMPLib;

    using WMPLib;

    public partial class WindowsMediaPlayerForm : Form
    {
        public WindowsMediaPlayerForm()
        {
            InitializeComponent();
        }

        public AxWindowsMediaPlayer MediaPlayer
        {
            get
            {
                return this.axWindowsMediaPlayer1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.MediaPlayer.PlayStateChange += this.PlayerOnPlayStateChange;
            this.MediaPlayer.MediaError += this.PlayerOnMediaError;
            this.MediaPlayer.uiMode = "none";
            this.MediaPlayer.URL = WindowsMediaPlayerGame.VideoPath;
            this.MediaPlayer.Ctlcontrols.play();
        }

        private void PlayerOnMediaError(object sender, _WMPOCXEvents_MediaErrorEvent e)
        {
        }

        private void PlayerOnPlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (this.MediaPlayer.playState == WMPPlayState.wmppsPlaying)
            {
                this.MediaPlayer.fullScreen = true;
            }
        }
    }
}
