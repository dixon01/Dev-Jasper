// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisFileReader.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisFileReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Simulation
{
    using System;
    using System.IO;

    using Gorba.Common.Configuration.Protran.Ibis;

    /// <summary>
    /// Base class for reading IBIS simulation files.
    /// </summary>
    public abstract class IbisFileReader : IDisposable
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisFileReader"/> class.
        /// </summary>
        /// <param name="config">
        /// The simulation config.
        /// </param>
        protected IbisFileReader(SimulationConfig config)
        {
            this.Config = config;
        }

        /// <summary>
        /// Gets or sets the time to wait after reading the <see cref="CurrentTelegram"/>.
        /// </summary>
        public TimeSpan CurrentWaitTime { get; protected set; }

        /// <summary>
        /// Gets or sets the current telegram bytes.
        /// </summary>
        public byte[] CurrentTelegram { get; protected set; }

        /// <summary>
        /// Gets the config for this reader.
        /// </summary>
        protected SimulationConfig Config { get; private set; }

        /// <summary>
        /// Creates a new implementation of this class depending on the given config.
        /// </summary>
        /// <param name="config">
        /// The simulation config.
        /// </param>
        /// <returns>
        /// A new instance of a subclass of <see cref="IbisFileReader"/>
        /// </returns>
        public static IbisFileReader Create(SimulationConfig config)
        {
            if (config.SimulationFile.EndsWith(".ism", StringComparison.InvariantCultureIgnoreCase))
            {
                return new IsmFileReader(config);
            }

            if (config.SimulationFile.EndsWith(".pro.csv", StringComparison.InvariantCultureIgnoreCase))
            {
                return new WbProCsvFileReader(config);
            }

            if (config.SimulationFile.EndsWith(".pcap", StringComparison.InvariantCultureIgnoreCase))
            {
                return new PcapUdpFileReader(config);
            }

            return new DefaultFileReader(config);
        }

        /// <summary>
        /// Closes this reader.
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Opens this reader and opens the underlying file.
        /// </summary>
        public virtual void Open()
        {
            this.reader = new StreamReader(this.Config.SimulationFile);
        }

        /// <summary>
        /// Closes the underlying file.
        /// </summary>
        public virtual void Close()
        {
            if (this.reader == null)
            {
                return;
            }

            this.reader.Close();
        }

        /// <summary>
        /// Resets this reader to again read from the beginning.
        /// This implementation simply calls <see cref="Close"/>
        /// followed by <see cref="Open"/>
        /// </summary>
        /// <returns>
        /// true if this reader was reset successfully.
        /// </returns>
        public virtual bool Reset()
        {
            this.Close();
            this.Open();
            return true;
        }

        /// <summary>
        /// Reads the next telegram from the file.
        /// </summary>
        /// <returns>
        /// true if a new telegram was found.
        /// </returns>
        public abstract bool ReadNext();

        /// <summary>
        /// Reads a single line from the underlying file.
        /// </summary>
        /// <returns>
        /// The read line or null if the end of the file was reached.
        /// </returns>
        protected string ReadLine()
        {
            return this.reader.ReadLine();
        }
    }
}
