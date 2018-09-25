// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimplePartBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimplePartBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    /// <summary>
    /// Base class for a part created by <see cref="SimpleTextFactory"/>.
    /// </summary>
    public abstract class SimplePartBase : IPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePartBase"/> class.
        /// </summary>
        /// <param name="blink">
        /// The blink.
        /// </param>
        internal SimplePartBase(bool blink)
        {
            this.Blink = blink;
        }

        /// <summary>
        /// Gets a value indicating whether this part should blink.
        /// </summary>
        public bool Blink { get; private set; }

        /// <summary>
        /// Duplicates this part if necessary.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// This object.
        /// </returns>
        public virtual IPart Duplicate()
        {
            return this;
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