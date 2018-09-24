// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioPlayer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioPlayer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Playback
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.AudioRenderer;

    using NLog;

    /// <summary>
    /// Class that plays a list of items (files, TTS and pause) in sequence.
    /// </summary>
    public class AudioPlayer : IDisposable, IPlaylist
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<AudioItemBase> parts = new List<AudioItemBase>();

        private readonly AudioRendererConfig config;

        private IAudioIOHandler audioHandler;

        private int playIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPlayer"/> class.
        /// </summary>
        /// <param name="config">
        /// The audio renderer configuration.
        /// </param>
        public AudioPlayer(AudioRendererConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Event that is fired when this player has completed playing.
        /// This event might also be risen when <see cref="Stop"/> is called.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Add an audio file to play.
        /// </summary>
        /// <param name="fileName">
        /// The absolute file name.
        /// </param>
        /// <param name="volume">
        /// The audio volume at which to play to file (0..100).
        /// </param>
        public void AddFile(string fileName, int volume)
        {
            this.CheckCanAdd();

            this.parts.Add(new FileItem(fileName, volume));
            Logger.Debug("Add File Called - file:" + fileName + "  with volume " + volume);
        }

        /// <summary>
        /// Add a text to be spoken.
        /// </summary>
        /// <param name="voice">
        /// The TTS voice to use.
        /// </param>
        /// <param name="text">
        /// The text to speak.
        /// </param>
        /// <param name="volume">
        /// The audio volume at which to speak the prompt (0..100).
        /// </param>
        public void AddSpeech(string voice, string text, int volume)
        {
            this.CheckCanAdd();

            if (this.config.TextToSpeech != null)
            {
                Logger.Info("AddSpeech Called - voice:" + voice + "  text " + text + " volume =" + volume);
                this.parts.Add(SpeechItemBase.Create(this.config.TextToSpeech.Api, voice, text, volume));
				return;
		    }           
			
			Logger.Warn("Composer send a TTS screen, but TTS engine is not configured");
		}

        /// <summary>
        /// Add a pause during playback.
        /// </summary>
        /// <param name="duration">
        /// The duration of the pause.
        /// </param>
        public void AddPause(TimeSpan duration)
        {
            this.CheckCanAdd();

            this.parts.Add(new PauseItem(duration));
        }

        /// <summary>
        /// Start the playback of this list.
        /// This method can only be called once.
        /// </summary>
        /// <param name="handler">
        /// Handler for audio I/O values.
        /// </param>
        public void Start(IAudioIOHandler handler)
        {
            if (this.playIndex >= 0)
            {
                throw new NotSupportedException("Can't start player twice");
            }

            this.audioHandler = handler;
            Logger.Debug("Starting player");
            this.PlayNext();
        }

        /// <summary>
        /// Stop the playback of this list.
        /// This method can only be called once.
        /// After calling this method, this object has to be disposed, it can't be reused.
        /// </summary>
        public void Stop()
        {
            var index = this.playIndex;
            if (index >= this.parts.Count)
            {
                return;
            }

            this.playIndex = this.parts.Count;
            var part = this.parts[index];
            part.Completed -= this.PartOnCompleted;
            try
            {
                part.Stop();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't stop " + part.GetType().Name);
            }

            this.RaiseCompleted(EventArgs.Empty);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Stop();
            foreach (var part in this.parts)
            {
                part.Dispose();
            }
        }

        /// <summary>
        /// Raises the <see cref="Completed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCompleted(EventArgs e)
        {
            Completed?.Invoke(this, e);
        }

        private void CheckCanAdd()
        {
            if (this.playIndex >= 0)
            {
                throw new NotSupportedException("Can't add items while playing");
            }
        }

        private void PlayNext()
        {
            if (this.playIndex >= 0)
            {
                this.parts[this.playIndex].Completed -= this.PartOnCompleted;
            }

            if (++this.playIndex >= this.parts.Count)
            {
                this.RaiseCompleted(EventArgs.Empty);
                return;
            }

            var part = this.parts[this.playIndex];
            var playback = part as PlaybackItemBase;
            if (playback != null)
            {
                this.audioHandler.SpeakerVolume = playback.Volume;
            }

            part.Completed += this.PartOnCompleted;
            try
            {
                part.Start();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't start " + part.GetType().Name, ex);
                this.PlayNext();
            }
        }

        private void PartOnCompleted(object sender, EventArgs eventArgs)
        {
            this.PlayNext();
        }
    }
}
