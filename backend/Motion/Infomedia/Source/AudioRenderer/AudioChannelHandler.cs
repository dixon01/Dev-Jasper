// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioChannelHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioChannelHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Gorba.Common.Configuration.Infomedia.AudioRenderer;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.AudioRenderer.Playback;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using NLog;

    /// <summary>
    /// The audio channel handler.
    /// </summary>
    public class AudioChannelHandler
    {
        private readonly object renderManagerLock = new object();

        private AudioChannelConfig config;

        private AudioIOHandler audioIOHandler;

        private RenderManagerFactory renderManagerFactory;

        private Logger logger;

        private ScreenRootRenderManager<IAudioRenderContext> currentRenderManager;

        private PlayerEngine playerEngine;

        private AudioRendererConfig audioRendererConfig;

        /// <summary>
        /// Gets the id of the audio channel.
        /// </summary>
        public string Id { get; private set; }

#if __UseLuminatorTftDisplay
        /// <summary>
        /// Gets the AudioIOHandler, for Luminator unit testing
        /// </summary>
        public AudioIOHandler AudioIOHandler { get { return audioIOHandler; } }
#endif

        /// <summary>
        /// Configures the audio channel.
        /// </summary>
        /// <param name="audioChannelConfig">
        /// The audio channel config.
        /// </param>
        /// <param name="audioConfig">
        /// The configuration for the IO port for volume
        /// </param>
        public void Configure(AudioChannelConfig audioChannelConfig, AudioRendererConfig audioConfig)
        {
            this.renderManagerFactory = new RenderManagerFactory();
            this.config = audioChannelConfig;
            this.audioRendererConfig = audioConfig;
            this.Id = this.config.Id;            
            this.audioIOHandler = new AudioIOHandler(this.audioRendererConfig.IO, audioChannelConfig);
            this.logger = LogManager.GetLogger(this.GetType().FullName + "-" + this.Id);
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        /// <param name="engine">The player engine for the audio</param>
        public void Start(PlayerEngine engine)
        {
            this.playerEngine = engine;
            this.audioIOHandler.Start();
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public void Stop()
        {
            this.audioIOHandler.Dispose();
        }

        /// <summary>
        /// Loads a new screen from the given <paramref name="change"/> (if the change is for us).
        /// </summary>
        /// <param name="change">
        /// The change.
        /// </param>
        /// <returns>
        /// True if the change was meant for us and we have handled it. Otherwise false.
        /// </returns>
        public bool LoadScreen(ScreenChange change)
        {
            if (change.Screen.Type != PhysicalScreenType.Audio || change.Screen.Id != this.config.Id)
            {
                return false;
            }

            this.logger.Info("Screen changed. New screen will be shown: #{0}", change.ScreenRoot.Id);
            var root = (ScreenRoot)change.ScreenRoot.Clone();
            this.logger.Trace("New screen: {0}", root);

            var manager = this.renderManagerFactory.CreateRenderManager(root);

            this.logger.Trace("Created new root render manager");

            this.ChangeRenderManager(manager);

            this.logger.Trace("Finished switching to new screen");
            return true;
        }

        /// <summary>
        /// Updates the audio channel handled by this handler with the given updates.
        /// </summary>
        /// <param name="updates">
        /// The screen updates.
        /// </param>
        public void UpdateScreen(IList<ScreenUpdate> updates)
        {
            this.logger.Trace("Updating render manager with {0} updates", updates.Count);

            lock (this.renderManagerLock)
            {
                if (this.currentRenderManager == null)
                {
                    return;
                }

                if (!this.currentRenderManager.Update(updates))
                {
                    this.logger.Trace("Render manager not updated");
                    return;
                }
            }

            this.Render();

            this.logger.Trace("Finished updating render manager");
        }

        /// <summary>
        /// Updates system volume change.
        /// </summary>
        /// <param name="volume">
        /// The volume.
        /// </param>
        public void UpdateSystemVolume(int volume)
        {
            this.audioIOHandler.SystemVolume = volume;
        }
        private void ChangeRenderManager(ScreenRootRenderManager<IAudioRenderContext> manager)
        {
            lock (this.renderManagerLock)
            {
                if (this.currentRenderManager != null)
                {
                    this.currentRenderManager.Dispose();
                }

                this.currentRenderManager = manager;
            }

            this.Render();
        }

        private void Render()
        {
            var manager = this.currentRenderManager;
            if (manager == null)
            {
                return;
            }

            // fix for the issue that playback would never stop on Windows Embedded 8 Standard
            SynchronizationContext.SetSynchronizationContext(null);

            this.logger.Debug("Rendering next audio output");
            var context = new AudioRenderContext(this.audioRendererConfig);
            try
            {
                manager.Update(context);
                manager.Render(1, context);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex,"Couldn't render audio output, not playing anything");
                return;
            }

            foreach (var pair in context)
            {
                this.playerEngine.Enqueue(pair.Key, pair.Value, this.audioIOHandler);
            }
        }

        private class AudioRenderContext : IAudioRenderContext, IEnumerable<KeyValuePair<int, AudioPlayer>>
        {
            private readonly SortedDictionary<int, AudioPlayer> audioPlayers = new SortedDictionary<int, AudioPlayer>();

            private readonly AudioRendererConfig config;

            public AudioRenderContext(AudioRendererConfig config)
            {
                this.config = config;
                this.MillisecondsCounter = TimeProvider.Current.TickCount;
            }

            public long MillisecondsCounter { get; private set; }

            public IPlaylist GetPlaylist(int priority)
            {
                AudioPlayer player;
                if (!this.audioPlayers.TryGetValue(priority, out player))
                {
                    player = new AudioPlayer(this.config);
                    this.audioPlayers[priority] = player;
                }

                return player;
            }

            public IEnumerator<KeyValuePair<int, AudioPlayer>> GetEnumerator()
            {
                return this.audioPlayers.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}