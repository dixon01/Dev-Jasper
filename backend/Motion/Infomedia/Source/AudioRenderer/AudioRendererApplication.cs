// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioRendererApplication.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioRendererApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Reflection;

namespace Gorba.Motion.Infomedia.AudioRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.AudioRenderer;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.AudioRenderer.Playback;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using NAudio.Wave;

    /// <summary>
    /// The audio renderer application.
    /// </summary>
    public class AudioRendererApplication : ApplicationBase
    {
        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        private readonly ITimer initialRequestTimer;

        private readonly List<AudioChannelHandler> audioChannels = new List<AudioChannelHandler>();

        private readonly List<AudioChannelHandler> missingAudioChannelRenderers = new List<AudioChannelHandler>();
        private readonly PlayerEngine playerEngine;
        private int systemVolume;

        private DirectSoundOut wavePlayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRendererApplication"/> class.
        /// </summary>
        public AudioRendererApplication()
        {
            this.initialRequestTimer = TimerFactory.Current.CreateTimer("InitialRequest");
            this.initialRequestTimer.Interval = TimeSpan.FromSeconds(3);
            this.initialRequestTimer.AutoReset = true;
            this.initialRequestTimer.Elapsed += this.InitialRequestTimerOnElapsed;
#if DEBUG
            // remove!!!!!!!! KEVIN
      //      Debugger.Launch();
#endif
            var configMgr = new ConfigManager<AudioRendererConfig>();
            configMgr.FileName = PathManager.Instance.GetPath(FileType.Config, "AudioRenderer.xml");
            configMgr.EnableCaching = true;
            configMgr.XmlSchema = AudioRendererConfig.Schema;
            var config = configMgr.Config;
            Logger.Trace("*** AudioRendererApplication() has {0} audio channels. ***", config.AudioChannels.Count);
            foreach (var audioChannelConfig in config.AudioChannels)
            {
                var handler = new AudioChannelHandler();
                Logger.Trace("AudioRendererApplication() before handler.Configure().");
                handler.Configure(audioChannelConfig, config);
                this.audioChannels.Add(handler);
            }

            this.playerEngine = new PlayerEngine();

            // We need to playback some short voice in order to initialize the  Audio Codec
            this.InitializeAudioCodec();
            if (config.TextToSpeech != null && config.TextToSpeech.Api == TextToSpeechApi.Acapela)
            {
                Logger.Trace("AudioRendererApplication() initializing AcapelaHelper. HintPath={0}", config.TextToSpeech.HintPath);
                AcapelaHelper.Initialize(config.TextToSpeech.HintPath);
            }
            this.systemVolume = config.IO.VolumePort.Value;
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// </summary>
        protected override void DoRun()
        {
            MessageDispatcher.Instance.Subscribe<ScreenChanges>(this.OnScreenChange);
            MessageDispatcher.Instance.Subscribe<VolumeSettingsMessage>(this.OnAudioSwitchSettingsRequest);

            this.initialRequestTimer.Enabled = true;

            foreach (var audioChannelHandler in this.audioChannels)
            {
                Logger.Trace("DoRun() calling Start() for audio channel: {0}.", audioChannelHandler);
                audioChannelHandler.Start(this.playerEngine);
                lock (this.missingAudioChannelRenderers)
                {
                    this.missingAudioChannelRenderers.Add(audioChannelHandler);
                }
            }

            this.SetRunning();

            this.runWait.WaitOne();

            foreach (var audioChannelHandler in this.audioChannels)
            {
                audioChannelHandler.Stop();
            }

            this.audioChannels.Clear();
        }

        private void OnAudioSwitchSettingsRequest(object sender, MessageEventArgs<VolumeSettingsMessage> e)
        {
            Logger.Debug("*** Request for Audio Hardware Settings *****");
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="DoRun"/>.
        /// </summary>
        protected override void DoStop()
        {
            MessageDispatcher.Instance.Unsubscribe<ScreenChanges>(this.OnScreenChange);
            MessageDispatcher.Instance.Unsubscribe<VolumeSettingsMessage>(this.OnAudioSwitchSettingsRequest);
            this.initialRequestTimer.Enabled = false;

            this.runWait.Set();

            if (AcapelaHelper.Available)
            {
#if __UseLuminatorTftDisplay
                AcapelaHelper.Engine.Dispose();
#endif
            }
        }

        private void OnScreenChange(object sender, MessageEventArgs<ScreenChanges> e)
        {
            this.Logger.Debug("OnScreenChange with {0} changes", e.Message.Changes.Count);
            var screenLoaded = false;

            lock (this.missingAudioChannelRenderers)
            {
                foreach (var screenChange in e.Message.Changes)
                {
                    if (screenChange.Screen.Type == PhysicalScreenType.Audio)
                    {
                        screenLoaded |= this.LoadScreen(screenChange);
                    }
                }
            }

            e.Message.Updates.Sort((a, b) => a.RootId.CompareTo(b.RootId));

            if (e.Message.Updates.Count > 0)
            {
                foreach (var audioChannelHandler in this.audioChannels)
                {
                    audioChannelHandler.UpdateScreen(e.Message.Updates);
                }
            }

            if (!screenLoaded)
            {
                return;
            }

            this.initialRequestTimer.Enabled = this.missingAudioChannelRenderers.Count > 0;
        }

        private bool LoadScreen(ScreenChange change)
        {
            foreach (var audioChannelHandler in this.audioChannels)
            {
                Logger.Trace("LoadScreen() called for audio channel: {0}.", audioChannelHandler);
                if (audioChannelHandler.LoadScreen(change))
                {
                    Logger.Trace("missingAudioChannelRenderers.Remove() called.");
                    this.missingAudioChannelRenderers.Remove(audioChannelHandler);
                    return true;
                }
            }

            return false;
        }

        private void InitialRequestTimerOnElapsed(object sender, EventArgs args)
        {
            var request = new ScreenRequests();

            lock (this.missingAudioChannelRenderers)
            {
                foreach (var missingAudioChannelRenderer in this.missingAudioChannelRenderers)
                {
                    request.Screens.Add(
                        new ScreenRequest
                        {
                            ScreenId =
                                new ScreenId
                                {
                                    Type = PhysicalScreenType.Audio,
                                    Id = missingAudioChannelRenderer.Id
                                },
                            Width = 0,
                            Height = 0
                        });
                }
            }

            this.Logger.Debug("Requesting for {0} screens", request.Screens.Count);
            MessageDispatcher.Instance.Broadcast(request);
        }
		
        private void InitializeAudioCodec()
        {
            var reader = new Mp3FileReader(this.GetResourceStream("Resources.point1sec.mp3"));
            this.wavePlayer = new DirectSoundOut();
            this.wavePlayer.Init(reader);
            this.wavePlayer.PlaybackStopped += this.FinishedCodecInitialization;
            this.wavePlayer.Play();
        }

        private Stream GetResourceStream(string fileName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resname = asm.GetName().Name + "." + fileName;
            return asm.GetManifestResourceStream(resname);
        }

        private void FinishedCodecInitialization(object sender, EventArgs eventArgs)
        {
            this.wavePlayer.Stop();
            this.wavePlayer.Dispose();
            this.Logger.Info("Finished codec initialization", eventArgs.ToString());
        }
    }
}

