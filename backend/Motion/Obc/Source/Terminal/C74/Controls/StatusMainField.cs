// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusMainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusMainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The status main field.
    /// This control is not yet implemented since it is currently not accessible through the UI.
    /// </summary>
    public partial class StatusMainField : MainField, IStatusMainField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusMainField"/> class.
        /// </summary>
        public StatusMainField()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Updates this field with the given drive information.
        /// </summary>
        /// <param name="driveInfo">
        /// The drive information.
        /// </param>
        public void Update(IDriveInfo driveInfo)
        {
            // TODO: implement
        }

        /// <summary>
        /// Updates this field with the given GPS information.
        /// </summary>
        /// <param name="gpsInfo">
        /// The GPS information.
        /// </param>
        public void Update(IGpsInfo gpsInfo)
        {
            // TODO: implement
        }
    }
}
