// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioZoneTypes.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;

    ///// <summary>The canned message zone type.</summary>
    [Flags]
    public enum AudioZoneTypes
    {
        /// <summary>The none.</summary>
        None = 0,

        /// <summary>The interior.</summary>
        Interior = 0x1,

        /// <summary>The exterior.</summary>
        Exterior = 0x2,

        /// <summary>The both.</summary>
        Both = Interior | Exterior
    }
}