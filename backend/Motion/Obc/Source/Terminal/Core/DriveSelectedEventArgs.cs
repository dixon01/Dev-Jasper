// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveSelectedEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveSelectedEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The drive selected event arguments.
    /// </summary>
    public class DriveSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriveSelectedEventArgs"/> class.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="drivingSchool">
        /// The driving school flag.
        /// </param>
        /// <param name="additional">
        /// The additional course flag.
        /// </param>
        public DriveSelectedEventArgs(int index, bool drivingSchool, bool additional)
        {
            this.Index = index;
            this.DrivingSchool = drivingSchool;
            this.Additional = additional;
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the drive is a driving school.
        /// </summary>
        public bool DrivingSchool { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the drive is an additional course.
        /// </summary>
        public bool Additional { get; private set; }
    }
}