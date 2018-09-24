// PresentationPlayLogging
// PresentationPlayLogging.Core
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.Core.Models
{
    using System;

    /// <summary>The add infotransit presentation info entry.</summary>
    [Serializable]
    public class AddInfotransitPresentationInfoEntry
    {
        public AddInfotransitPresentationInfoEntry(InfotransitPresentationInfo infotransitPresentationInfo)
        {
            this.InfotransitPresentationInfo = infotransitPresentationInfo;
        }

        /// <summary>Gets or sets the infotransit presentation info.</summary>
        public InfotransitPresentationInfo InfotransitPresentationInfo { get; set; }
    }
}