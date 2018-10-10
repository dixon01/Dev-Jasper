// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioComposerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioComposerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Base class for all composers that are representing a
    /// <see cref="AudioItemBase"/>. This class takes care about
    /// the generic aspects of an audio element.
    /// </summary>
    /// <typeparam name="TElement">
    /// The type of element used by this composer.
    /// </typeparam>
    public partial class AudioComposerBase<TElement>
    {
        /// <summary>
        /// Event for subclasses that is fired whenever the
        /// <see cref="AudioElementBase.EnabledProperty"/> changes
        /// </summary>
        protected event EventHandler EnabledChanged;

        /// <summary>
        /// Gets the parent composer or null if it is not an <see cref="AudioOutputComposer"/>.
        /// </summary>
        protected AudioOutputComposer OutputParent { get; private set; }

        /// <summary>
        /// Method to check if the enabled property on this element is true.
        /// This method also checks the parent's enabled property.
        /// If no dynamic "Enabled" property is defined, this method returns true.
        /// Subclasses can override this method to provide additional
        /// evaluation (e.g. check if files are available or other conditions
        /// are met).
        /// </summary>
        /// <returns>
        /// true if there is no property defined or if the property evaluates to true is valid.
        /// </returns>
        protected virtual bool IsEnabled()
        {
            if (this.OutputParent != null && !this.OutputParent.IsEnabled())
            {
                return false;
            }

            return this.HandlerEnabled.BoolValue;
        }

        /// <summary>
        /// Raises the <see cref="EnabledChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseEnabledChanged(EventArgs e)
        {
            var handler = this.EnabledChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        partial void Initialize()
        {
            this.OutputParent = this.Parent as AudioOutputComposer;
            if (this.OutputParent != null)
            {
                this.OutputParent.EnabledChanged += this.OnEnabledChanged;
            }
        }

        partial void Deinitialize()
        {
            if (this.OutputParent != null)
            {
                this.OutputParent.EnabledChanged -= this.OnEnabledChanged;
            }
        }

        private void OnEnabledChanged(object sender, EventArgs e)
        {
            this.RaiseEnabledChanged(e);
        }
    }
}