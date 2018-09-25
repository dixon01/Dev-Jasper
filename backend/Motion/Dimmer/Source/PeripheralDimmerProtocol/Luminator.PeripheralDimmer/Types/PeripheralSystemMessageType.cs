// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralSystemMessageType.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The peripheral system message type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Types
{
    /// <summary>The peripheral system message type.</summary>
    public enum PeripheralSystemMessageType : byte
    {
        /// <summary>The unknown.</summary>
        Unknown = 0, 

        /// <summary>The infotransite tft dimmer.</summary>
        InfotransiteTftDimmer = 0x6, 

        /// <summary>The audio generation 3.</summary>
        AudioGeneration3 = 0x7
    }
}