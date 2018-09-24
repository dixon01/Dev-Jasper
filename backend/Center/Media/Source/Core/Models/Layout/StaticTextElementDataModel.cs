// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticTextElementDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the properties of a static text layout element.
    /// </summary>
    [XmlRoot("StaticText")]
    [Serializable]
    public class StaticTextElementDataModel : TextElementDataModel
    {
    }
}
