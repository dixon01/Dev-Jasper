// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareManagerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.HardwareManager.Gps;
    using Gorba.Common.Configuration.HardwareManager.Mgi;
    using Gorba.Common.Configuration.HardwareManager.Vdv301;

    /// <summary>
    /// The configuration of hardware manager
    /// </summary>
    [XmlRoot("HardwareManager")]
    [Serializable]
    public class HardwareManagerConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareManagerConfig"/> class.
        /// </summary>
        public HardwareManagerConfig()
        {
            this.BroadcastTimeChanges = true;
            this.Mgi = new MgiConfig();
            this.Settings = new List<HardwareManagerSetting>();
            this.Gps = new GpsConfig();
            this.Sntp = new SntpConfig();
            this.Vdv301 = new Vdv301Config();

#if __UseLuminatorTftDisplay
            this.Mgi.Enabled = false;
            if (this.Gps.Client != null)
            {                
                this.Gps.Client.Enabled = false;
            }

            this.Vdv301.Enabled = false;
#endif
        }

        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (
                    var input =
                        typeof(HardwareManagerConfig).Assembly.GetManifestResourceStream(
                            typeof(HardwareManagerConfig), "HardwareManager.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find HardwareManager.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this application should
        /// broadcast time changes to any other Hardware Managers.
        /// If this is enabled, this application will also listen to incoming
        /// time changes via Medi, but not redistribute those received over Medi.
        /// </summary>
        public bool BroadcastTimeChanges { get; set; }

        /// <summary>
        /// Gets or sets the MGI Topbox and Compact related configuration.
        /// </summary>
        public MgiConfig Mgi { get; set; }

        /// <summary>
        /// Gets or sets the GPS receiver configuration.
        /// </summary>
        [XmlElement("GPS")]
        public GpsConfig Gps { get; set; }

        /// <summary>
        /// Gets or sets the general SNTP (network time) configuration.
        /// </summary>
        [XmlElement("SNTP")]
        public SntpConfig Sntp { get; set; }

        /// <summary>
        /// Gets or sets the VDV 301 configuration.
        /// </summary>
        [XmlElement("VDV301")]
        public Vdv301Config Vdv301 { get; set; }

        /// <summary>
        /// Gets or sets the condition-based hardware manager settings.
        /// Only the first setting in the list that is valid will be taken.
        /// </summary>
        [XmlArrayItem("Setting")]
        public List<HardwareManagerSetting> Settings { get; set; }
    }
}
