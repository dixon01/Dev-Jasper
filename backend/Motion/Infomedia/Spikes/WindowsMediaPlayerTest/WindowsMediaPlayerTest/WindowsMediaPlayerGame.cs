// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Game1.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is the main type for your game
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WindowsMediaPlayerTest
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;

    using AxWMPLib;

    using WMPLib;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class WindowsMediaPlayerGame : GameBase
    {
        private readonly AxWindowsMediaPlayer player;

        private readonly WindowsMediaPlayerForm form;

        private bool wasFullScreen;

        public WindowsMediaPlayerGame()
        {
            this.form = new WindowsMediaPlayerForm();
            this.player = this.form.MediaPlayer;
            this.player.PlayStateChange += this.PlayerOnPlayStateChange;
            this.player.MediaError += this.PlayerOnMediaError;
        }

        protected override void PlayVideo()
        {
            this.wasFullScreen = this.graphics.IsFullScreen;

            this.player.uiMode = "none";
            this.player.URL = VideoPath;
            this.player.Ctlcontrols.play();
        }

        private void PlayerOnMediaError(object sender, _WMPOCXEvents_MediaErrorEvent e)
        {
        }

        private void PlayerOnPlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            Debug.WriteLine("{0:HH:mm:ss.fff} State Change: {1}", DateTime.Now, (WMPPlayState)e.newState);
            switch (this.player.playState)
            {
                case WMPPlayState.wmppsPlaying:
                    if (this.graphics.IsFullScreen)
                    {
                        this.player.Ctlcontrols.pause();
                        this.graphics.ToggleFullScreen();
                        this.player.Ctlcontrols.play();
                    }

                    this.player.fullScreen = true;
                    break;
                case WMPPlayState.wmppsStopped:
                    if (this.wasFullScreen)
                    {
                        this.wasFullScreen = false;
                        this.graphics.ToggleFullScreen();
                    }

                    break;
            }
        }
    }
}
