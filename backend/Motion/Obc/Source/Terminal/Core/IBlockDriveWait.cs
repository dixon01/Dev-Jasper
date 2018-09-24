// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBlockDriveWait.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBlockDriveWait type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The block drive wait field interface.
    /// </summary>
    public interface IBlockDriveWait : IMainField
    {
        /// <summary>
        /// Initializes the field by setting its data.
        /// </summary>
        /// <param name="destinationText">
        /// The destination text.
        /// </param>
        /// <param name="startText">
        /// The start text.
        /// </param>
        /// <param name="driveTimeStart">
        /// The drive time start.
        /// </param>
        void Init(string destinationText, string startText, DateTime driveTimeStart);
    }
}