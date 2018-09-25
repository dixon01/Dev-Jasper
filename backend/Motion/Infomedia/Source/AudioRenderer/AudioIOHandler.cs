// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioIOHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioIOHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Gorba.Common.Configuration.Infomedia.AudioRenderer;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="IAudioIOHandler"/> that uses GIOoM I/O.
    /// </summary>
    public class AudioIOHandler : IAudioIOHandler, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IOConfig config;

        private readonly List<PortListener> speakerPortListeners;

        private readonly List<IOPortConfig> speakerIOPortConfigs;

        private PortListener volumePort;

        private bool speakerEnabled;

        private int speakerVolume;

        private bool resetVolume;

        private int systemVolume;
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioIOHandler"/> class.
        /// </summary>
        /// <param name="config">
        ///     The config.
        /// </param>
        /// <param name="audioChannelConfig">The audio channel configuration</param>
        public AudioIOHandler(IOConfig config, AudioChannelConfig audioChannelConfig)
        {
            // Eliminate Nulls, Test for Null or Empty string later
            if (config.VolumePort.Application == null)
            {
                config.VolumePort.Application = string.Empty;
            }
            if (config.VolumePort.Unit == null)
            {
                config.VolumePort.Unit = string.Empty;
            }
            // End 

            var portNames = audioChannelConfig.SpeakerPorts.Aggregate(string.Empty, (current, speakerPort) => current + (speakerPort.PortName + ","));
            Logger.Info("AudioChannelHandler Construct speakerPortNames = {0}", portNames);
            
            this.config = config;
            Logger.Info("AudioChannelHandler Construct AudioIOHandler for VolumePortName = {0}, Application = {1}", config.VolumePort?.PortName, config.VolumePort?.Application);
            this.systemVolume = config.VolumePort.Value;
			this.speakerIOPortConfigs = audioChannelConfig.SpeakerPorts;
            this.speakerPortListeners = new List<PortListener>();            
        }

        /// <summary>
        /// Gets or sets a value indicating whether the speaker should be enabled.
        /// </summary>
        public bool SpeakerEnabled
        {
            get
            {
                return this.speakerEnabled;
            }

            set
            {

                if (this.speakerEnabled == value)
                {
                    Logger.Info("Setting property SpeakerEnabled ignored, already true : speakerEnabled == {0}", value);
                    return;
                }

                Logger.Info("Setting property SpeakerEnabled Setting speaker to {0}", value);
                this.speakerEnabled = value;
                foreach (var speakerPort in this.speakerPortListeners)
                {
                    if (speakerPort != null)
                    {
                        var name = speakerPort.Name;
                        Logger.Info("Setting speaker Name {0}, to speakerEnabled = {1}", name, this.speakerEnabled);
                        speakerPort.Value = FlagValues.GetValue(value);

                        // Infotransite LTG
                        var playbackEvent = AudioPlaybackEvent.CreateAudioPlaybackEvent(name, this.speakerEnabled);
                        MessageDispatcher.Instance.Send(new MediAddress(MessageDispatcher.Instance.LocalAddress.Unit, "Protran"), playbackEvent);
                        // end
                    }
                }

                if (!this.speakerEnabled)
                {
                    this.resetVolume = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current system speaker volume (0..100).
        /// </summary>
        public int SpeakerVolume
        {
            get
            {
                return this.speakerVolume;
            }

            set
            {
                if (this.speakerVolume == value && !this.resetVolume)
                {
                    return;
                }

                Logger.Info("Setting speaker volume to {0}", value);
                this.resetVolume = false;
                this.speakerVolume = value;
                if (this.volumePort != null && this.volumePort.Port != null)
                {
                    this.volumePort.Value = this.volumePort.Port.CreateValue(value);

                    // Infotransite
                    //var playbackEvent = new AudioPlaybackEvent() { SpeakerVolume = value };
                    //MessageDispatcher.Instance.Send(new MediAddress(MessageDispatcher.Instance.LocalAddress.Unit, "Protran"), playbackEvent);
                    // end
                }
            }
        }
		
        /// <summary>
        /// Gets or sets the system volume.
        /// </summary>
        public int SystemVolume
        {
            get
            {
                return this.systemVolume;
            }

            set
            {
                if (!this.speakerEnabled)
                {
                    this.systemVolume = value;
                    return;
                }

                if (this.systemVolume == value)
                {
                    return;
                }

                Logger.Debug("Setting system volume to {0}", value);
                this.systemVolume = value;
                if (this.volumePort != null && this.volumePort.Port != null)
                {
                    this.volumePort.Value = this.volumePort.Port.CreateValue(
                        (int)Math.Round(value * this.speakerVolume / 10.0));
                }
            }
        }

        /// <summary>
        /// Starts this handler by searching the ports.
        /// </summary>
        public void Start()
        {
            Logger.Debug("Start Enter");

            foreach (var portConfig in this.speakerIOPortConfigs)
            {
                var speakerPortName = portConfig.PortName;  // example 'Speaker1'

                Logger.Info("Adding Speaker Port Listener for Name:{0}", speakerPortName);
                this.speakerPortListeners.Add(StartPortListener(portConfig));
            }

            Logger.Info("Adding Volume Listener for Name:{0}", this.config?.VolumePort);
            this.volumePort = StartPortListener(this.config.VolumePort);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var speakerPort in this.speakerPortListeners)
            {
                if (speakerPort != null)
                {
                    speakerPort.Dispose();
                }
            }

            this.speakerPortListeners.Clear();
            this.speakerIOPortConfigs.Clear();

            if (this.volumePort != null)
            {
                this.volumePort.Dispose();
            }
        }

        private static PortListener StartPortListener(IOPortConfig cfg)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.PortName))
            {
                return null;
            }

            Logger.Info("StartPortListener Enter PortName={0} Application={1}, Unit={2}", cfg.PortName, cfg.Application, cfg.Unit);
            var portListener =
                new PortListener(
                    new MediAddress(
                        string.IsNullOrEmpty(cfg.Unit) ? MessageDispatcher.Instance.LocalAddress.Unit : cfg.Unit,
                        string.IsNullOrEmpty(cfg.Application) ? "*" : cfg.Unit),
                    cfg.PortName);
#if DEBUG
            if (portListener.Port == null)
            {
                Logger.Trace("listener.Port is Null and not yet set");
            }
            else
            {
                if (portListener.Port.Info == null)
                {
                    Logger.Trace("Port.Info is Null");
                }
            }
#endif         
            portListener.Start(TimeSpan.FromSeconds(5));
            return portListener;
        }
    }
}