// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiveStreamElementDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The live stream element data model.
    /// </summary>
    [XmlRoot("LiveStream")]
    [Serializable]
    public class LiveStreamElementDataModel : VideoElementDataModel
    {
    }
}