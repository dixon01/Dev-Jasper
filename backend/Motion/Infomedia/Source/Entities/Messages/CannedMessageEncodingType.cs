// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="CannedMessageEncodingType.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    /// <summary>The canned message encoding type.</summary>
    public enum CannedMessageEncodingType
    {
        /// <summary>The type is unknown.</summary>
        Unknown = 0, 

        /// <summary>The audio wav type.</summary>
        Wav = 1, 

        /// <summary>The audio mp3 type.</summary>
        Mp3 = 2, 

        /// <summary>The MPeg.</summary>
        Mpeg = 3
    }
}