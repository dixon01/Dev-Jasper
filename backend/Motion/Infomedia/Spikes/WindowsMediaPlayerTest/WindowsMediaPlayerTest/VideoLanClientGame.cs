namespace WindowsMediaPlayerTest
{
    using System;
    using System.Diagnostics;

    using AxAXVLC;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class VideoLanClientGame : GameBase
    {
        private readonly AxVLCPlugin2 vlcPlayer;

        private readonly AxVideoLanClientForm form;

        private bool wasFullScreen;

        public VideoLanClientGame()
        {
            this.form = new AxVideoLanClientForm();
            this.vlcPlayer = this.form.MediaPlayer;
            this.vlcPlayer.MediaPlayerPlaying += this.VlcPlayerOnMediaPlayerPlaying;
            this.vlcPlayer.MediaPlayerEndReached += this.VlcPlayerOnMediaPlayerEndReached;
        }

        protected override void PlayVideo()
        {
            this.wasFullScreen = this.graphics.IsFullScreen;

            this.vlcPlayer.playlist.clear();
            this.vlcPlayer.playlist.add(new Uri(VideoPath).ToString());
            this.vlcPlayer.playlist.playItem(0);

        }

        private void VlcPlayerOnMediaPlayerPlaying(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("{0:HH:mm:ss.fff} MediaPlayerPlaying", DateTime.Now);
            if (this.graphics.IsFullScreen)
            {
                this.graphics.ToggleFullScreen();
            }

            this.vlcPlayer.video.toggleFullscreen();
        }

        private void VlcPlayerOnMediaPlayerEndReached(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("{0:HH:mm:ss.fff} MediaPlayerEndReached", DateTime.Now);
            this.vlcPlayer.video.toggleFullscreen();
            if (this.wasFullScreen)
            {
                this.wasFullScreen = false;
                this.graphics.ToggleFullScreen();
            }
        }
    }
}