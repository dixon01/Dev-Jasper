// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoLanClientForm.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoLanClientForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WindowsMediaPlayerTest
{
    using System;
    using System.Windows.Forms;

    using AxAXVLC;

    // IMPORTANT: register COM object first:
    // C:\Windows\System32\regsvr32.exe "C:\Program Files (x86)\VideoLAN\VLC\axvlc.dll"
    // see also: http://wiki.videolan.org/Documentation:WebPlugin
    public partial class AxVideoLanClientForm : Form
    {
        public AxVideoLanClientForm()
        {
            this.InitializeComponent();
        }

        public AxVLCPlugin2 MediaPlayer
        {
            get
            {
                return this.axVLCPlugin21;
            }
        }

        private void Button1Click(object sender, EventArgs e)
        {
            this.axVLCPlugin21.playlist.clear();
            this.axVLCPlugin21.playlist.add(new Uri(WindowsMediaPlayerGame.VideoPath).ToString(), "the thing", ":fullscreen :http-continuous :ffmpeg-workaround-bugs=40");
            this.axVLCPlugin21.playlist.playItem(0);
            this.axVLCPlugin21.play += this.AxVlcPlugin21OnPlay;
            this.axVLCPlugin21.MediaPlayerTimeChanged += AxVlcPlugin21OnMediaPlayerTimeChanged;
            this.axVLCPlugin21.MediaPlayerPositionChanged += AxVlcPlugin21OnMediaPlayerPositionChanged;
            this.axVLCPlugin21.MediaPlayerBuffering += AxVlcPlugin21OnMediaPlayerBuffering;
            this.axVLCPlugin21.MediaPlayerForward += AxVlcPlugin21OnMediaPlayerForward;
            this.axVLCPlugin21.MediaPlayerOpening += AxVlcPlugin21OnMediaPlayerOpening;
            this.axVLCPlugin21.MediaPlayerPaused += AxVlcPlugin21OnMediaPlayerPaused;
            this.axVLCPlugin21.MediaPlayerPlaying += AxVlcPlugin21OnMediaPlayerPlaying;
            this.axVLCPlugin21.MediaPlayerEndReached += AxVlcPlugin21OnMediaPlayerEndReached;
        }

        private void AxVlcPlugin21OnMediaPlayerTimeChanged(object sender, DVLCEvents_MediaPlayerTimeChangedEvent dvlcEventsMediaPlayerTimeChangedEvent)
        {
        }

        private void AxVlcPlugin21OnMediaPlayerPositionChanged(object sender, DVLCEvents_MediaPlayerPositionChangedEvent dvlcEventsMediaPlayerPositionChangedEvent)
        {
        }

        private void AxVlcPlugin21OnMediaPlayerEndReached(object sender, EventArgs eventArgs)
        {
        }

        private void AxVlcPlugin21OnMediaPlayerPlaying(object sender, EventArgs eventArgs)
        {
            //this.axVLCPlugin21.video.toggleFullscreen();
        }

        private void AxVlcPlugin21OnMediaPlayerPaused(object sender, EventArgs eventArgs)
        {
        }

        private void AxVlcPlugin21OnMediaPlayerOpening(object sender, EventArgs eventArgs)
        {
        }

        private void AxVlcPlugin21OnMediaPlayerForward(object sender, EventArgs eventArgs)
        {
        }

        private void AxVlcPlugin21OnMediaPlayerBuffering(object sender, DVLCEvents_MediaPlayerBufferingEvent dvlcEventsMediaPlayerBufferingEvent)
        {
        }

        private void AxVlcPlugin21OnPlay(object sender, EventArgs eventArgs)
        {
            //this.axVLCPlugin21.video.toggleFullscreen();
        }
    }
}
