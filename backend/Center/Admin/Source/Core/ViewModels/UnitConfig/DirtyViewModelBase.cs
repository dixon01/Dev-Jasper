// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirtyViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirtyViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig
{
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework;

    /// <summary>
    /// Base class for all view models that have dirty management.
    /// </summary>
    public class DirtyViewModelBase : ViewModelBase, IDirty
    {
        private bool isDirty;

        /// <summary>
        /// Gets or sets a value indicating whether this instance has changes, making it <c>dirty</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }

            protected set
            {
                this.SetProperty(ref this.isDirty, value, () => this.IsDirty);
            }
        }

        /// <summary>
        /// Sets the <see cref="IDirty.IsDirty"/> flag. The default behavior only sets the flag on the current object.
        /// </summary>
        public void MakeDirty()
        {
            this.IsDirty = true;
        }

        /// <summary>
        /// Clears the <see cref="IsDirty"/> flag. The default behavior clears the flag on the current object and all
        /// its children.
        /// </summary>
        public virtual void ClearDirty()
        {
            this.IsDirty = false;
        }
    }
}
