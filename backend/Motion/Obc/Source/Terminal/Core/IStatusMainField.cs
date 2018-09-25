// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatusMainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStatusMainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// The status main field interface.
    /// </summary>
    public interface IStatusMainField : IMainField
    {
        /// <summary>
        /// Updates this field with the given drive information.
        /// </summary>
        /// <param name="driveInfo">
        /// The drive information.
        /// </param>
        void Update(IDriveInfo driveInfo);

        /// <summary>
        /// Updates this field with the given GPS information.
        /// </summary>
        /// <param name="gpsInfo">
        /// The GPS information.
        /// </param>
        void Update(IGpsInfo gpsInfo);
    }
}