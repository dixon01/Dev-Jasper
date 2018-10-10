// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Channels
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Ibis.Recording;

    using NLog;

    /// <summary>
    /// Factory class that creates all channels for a given configuration.
    /// </summary>
    public partial class ChannelFactory
    {
        private static readonly Logger Logger = LogHelper.GetLogger<ChannelFactory>();

        /// <summary>
        /// Creates all channel according to the given <see cref="configContext"/>.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        /// <returns>
        /// an array of channels that might be empty but never null.
        /// </returns>
        public virtual IEnumerable<IbisChannel> CreateChannels(IIbisConfigContext configContext)
        {
            var channel = this.CreateNewChannel(configContext);
            if (channel == null)
            {
                yield break;
            }

            channel.Recorder = this.CreateRecorder(configContext);
            yield return channel;
        }

        /// <summary>
        /// Creates a recorder for the given <see cref="configContext"/>.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        /// <returns>
        /// a new recorder or null if none was defined.
        /// </returns>
        protected IRecorder CreateRecorder(IIbisConfigContext configContext)
        {
            var recordingConfig = configContext.Config.Recording;
            if (recordingConfig == null || !recordingConfig.Active)
            {
                return null;
            }

            switch (recordingConfig.Format)
            {
                case RecordingFormat.Protran:
                    return new TimestampIbisRecorder(configContext);

                case RecordingFormat.Gismo:
                    // for the moment, the Gismo format is not supported.
                    // so, here Protran has to log this missing and then exit.
                    Logger.Info("Gismo format not supported.");
                    Logger.Info("Please, set the parameter \"ForGismo\" to false.");
                    Logger.Info("The application will exit now.");
                    ApplicationHelper.Exit(1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }

        private IbisChannel CreateNewChannel(IIbisConfigContext configContext)
        {
            var config = configContext.Config;
            switch (config.Sources.Active)
            {
                case IbisSourceType.None:
                    return null;
                case IbisSourceType.Simulation:
                    if (config.Sources.Simulation == null)
                    {
                        return null;
                    }

                    return new SimulationChannel(configContext);
                case IbisSourceType.SerialPort:
                    if (config.Sources.SerialPort == null)
                    {
                        return null;
                    }

                    return new IbisSerialChannel(configContext);
                case IbisSourceType.UDPServer:
                    if (config.Sources.UdpServer == null)
                    {
                        return null;
                    }

                    return new UdpServerChannel(configContext);
                case IbisSourceType.JSON:
                    return this.CreateJsonIbisChannel(configContext);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
