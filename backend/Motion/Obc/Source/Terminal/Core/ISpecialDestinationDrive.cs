// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpecialDestinationDrive.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISpecialDestinationDrive type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// The special destination drive main field.
    /// </summary>
    public interface ISpecialDestinationDrive : IMainField
    {
        /// <summary>
        /// Initializes this field.
        /// </summary>
        /// <param name="destinationText">
        /// The destination text.
        /// </param>
        void Init(string destinationText);
    }
}