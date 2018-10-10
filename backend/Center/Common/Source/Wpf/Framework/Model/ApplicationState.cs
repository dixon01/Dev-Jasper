// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Model
{
    using System.Runtime.Serialization;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Startup;

    /// <summary>
    /// Defines global properties identifying the state of the application.
    /// </summary>
    [DataContract]
    public class ApplicationState : ViewModelBase, IApplicationState
    {
        private bool isDirty;

        private ApplicationOptions applicationOptions;

        private DisplayContext displayContext;

        /// <summary>
        /// Gets or sets the display context.
        /// </summary>
        /// <remarks>Access is not thread-safe.</remarks>
        public DisplayContext DisplayContext
        {
            get
            {
                return this.displayContext ?? (this.displayContext = new DisplayContext());
            }

            set
            {
                this.displayContext = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has changes, making it <c>dirty</c>.
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

            private set
            {
                this.SetProperty(ref this.isDirty, value, () => this.IsDirty);
            }
        }

        /// <summary>
        /// Gets or sets the application options.
        /// </summary>
        [DataMember(Name = "Options")]
        public ApplicationOptions Options
        {
            get
            {
                return this.applicationOptions;
            }

            set
            {
                this.SetProperty(ref this.applicationOptions, value, () => this.Options);
            }
        }

        /// <summary>
        /// Sets the <see cref="IsDirty"/> flag. The default behavior only sets the flag on the current object.
        /// </summary>
        public virtual void MakeDirty()
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