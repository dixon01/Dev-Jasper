// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComponentBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Engines
{
    using System;

    /// <summary>
    /// The base class for all components rendered with the AHDLC renderer.
    /// </summary>
    public abstract class ComponentBase : IDisposable
    {
        /// <summary>
        /// Gets or sets the x coordinate.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y coordinate.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the z-index.
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether visible.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="object"/> is equal to the current <see cref="object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.
        /// </param>
        public override bool Equals(object obj)
        {
            var other = obj as ComponentBase;
            if (other == null)
            {
                return false;
            }

            return this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height
                   && this.ZIndex == other.ZIndex && this.Visible == other.Visible;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Width.GetHashCode() ^ this.Height.GetHashCode()
                   ^ this.ZIndex.GetHashCode();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }
    }
}