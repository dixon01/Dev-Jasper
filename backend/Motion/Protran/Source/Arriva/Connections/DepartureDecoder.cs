// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DepartureDecoder.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva.Connections
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;

    using NLog;

    /// <summary>
    /// Objected tasked to XML serialize/deserialize the departures.xml
    /// files received from Arriva.
    /// </summary>
    public class DepartureDecoder
    {
        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The object tasked to serialize/deserialize object in XML and vice versa.
        /// </summary>
        private Configurator configurator;

        /// <summary>
        /// Gets the instance of the unique object that contains
        /// all the configurations for the Abu Dhabi project.
        /// </summary>
        [XmlIgnore]
        public DeparturesConfig DeparturesConfig { get; private set; }

        /// <summary>
        /// Decodes the departures.xml files and returns its
        /// deserialization, if possible.
        /// </summary>
        /// <param name="departuresFile">
        /// The absolute path of the departures.xml to be decided.
        /// </param>
        /// <returns>
        /// The decode.
        /// </returns>
        public DeparturesConfig Decode(string departuresFile)
        {
            this.configurator = new Configurator(departuresFile);

            while (true)
            {
                try
                {
                    using (new FileStream(departuresFile, FileMode.Open, FileAccess.Read))
                    {
                        Thread.Sleep(500);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex, "File is used by someone else. Wait for release");
                }
            }

            this.PerformDeserialization();

            if (this.DeparturesConfig == null)
            {
                throw new XmlException("Could not deserialize from " + departuresFile);
            }

            return this.DeparturesConfig;
        }

        private void PerformDeserialization()
        {
            try
            {
                this.DeparturesConfig = this.configurator.Deserialize<DeparturesConfig>();
            }
            catch (Exception ex)
            {
                Logger.Debug("Deserialization failed due to {0}", ex.Message);
            }
        }
    }
}
