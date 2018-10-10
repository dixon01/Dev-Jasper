// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenId.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenId type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// The screen id.
    /// </summary>
    public class ScreenId
    {
        /// <summary>
        /// Gets or sets the type of the screen.
        /// </summary>
        public PhysicalScreenType Type { get; set; }

        /// <summary>
        /// Gets or sets the id of the string.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}-{1}", this.Type, this.Id);
        }
    }
}