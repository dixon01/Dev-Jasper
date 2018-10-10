// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Playback
{
    using System.Threading;

    using NAudio.Wave;

    using NLog;

    /// <summary>
    /// Playback item that plays a single audio file (MP3, AIFF or WAV).
    /// </summary>
    internal class FileItem : PlaybackItemBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string fileName;

        private readonly IWavePlayer wavePlayer;

        private readonly WaveStream reader;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileItem"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The absolute file name.
        /// </param>
        /// <param name="volume">
        /// The audio volume at which to play to file (0..100).
        /// </param>
        public FileItem(string fileName, int volume)
            : base(volume)
        {
            this.fileName = fileName;
            this.wavePlayer = new DirectSoundOut();
            this.reader = new AudioFileReader(fileName);
            this.wavePlayer.Init(this.reader);
            this.wavePlayer.PlaybackStopped += this.WavePlayerOnPlaybackStopped;
        }

        /// <summary>
        /// Start playing this item.
        /// </summary>
        public override void Start()
        {
            Logger.Info("Playing {0}", this.fileName);
            this.wavePlayer.Play();
        }

        /// <summary>
        /// Immediately stop playing this item.
        /// </summary>
        public override void Stop()
        {
            this.wavePlayer.Stop();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            ThreadPool.QueueUserWorkItem(
                s =>
                    {
                        this.wavePlayer.PlaybackStopped -= this.WavePlayerOnPlaybackStopped;
                        this.wavePlayer.Stop();
                        this.reader.Dispose();
                        this.wavePlayer.Dispose();
                    });
        }

        private void WavePlayerOnPlaybackStopped(object sender, StoppedEventArgs stoppedEventArgs)
        {
            this.RaiseCompleted(stoppedEventArgs);
        }
    }
}