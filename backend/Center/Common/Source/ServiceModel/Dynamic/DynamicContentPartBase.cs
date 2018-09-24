// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicContentPartBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicContentPartBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Dynamic
{
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for all dynamically created content parts.
    /// </summary>
    [XmlInclude(typeof(RssFeedDynamicContentPart))]
    [XmlInclude(typeof(ScreenGateDynamicContentPart))]
    [XmlInclude(typeof(EPaperDynamicContentPart))]
    public abstract class DynamicContentPartBase
    {
    }
}