// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodecConfig.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Base class for all codec configuration objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for all codec configuration objects.
    /// </summary>
    [XmlInclude(typeof(Transcoder.Xml.XmlCodecConfig))]
    [XmlInclude(typeof(Transcoder.Bec.BecCodecConfig))]
    [XmlInclude(typeof(Transcoder.Dynamic.DynamicCodecConfig))]
    public class CodecConfig
    {
    }
}