// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UriSettings.cs" company="Luminator LTG">
//   Copyright © 2011-2018 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UriSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Models
{
    using System;
    using System.Xml.Serialization;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    /// <summary>The uri settings.</summary>
    [Serializable]
    public class UriSettings
    {
        public UriSettings()
        {            
        }

        public UriSettings(Uri hostUri)
        {
            this.HostUri = hostUri;
        }

        public UriSettings(string uri)
        {
            this.HostUri = new Uri(uri);    
        }

        [XmlIgnore]
        public Uri HostUri { get; set; }

        [XmlElement("HostUri")]
        public string UriXml
        {
            get
            {
                return this.HostUri != null ? this.HostUri.OriginalString : string.Empty;
            }

            set
            {
                this.HostUri = new Uri(value);
            }
        }
    }
}