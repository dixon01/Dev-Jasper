// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SectionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using NLog;

    /// <summary>
    /// Class that represents a section of a cycle.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item inside this section. To be defined by subclasses.
    /// </typeparam>
    public abstract class SectionBase<T> : IDisposable, IManageableObject
        where T : class
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private TimeSpan currentDuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionBase{T}"/> class.
        /// </summary>
        /// <param name="config">
        /// The step config.
        /// </param>
        /// <param name="context">
        /// The generic context.
        /// </param>
        protected SectionBase(SectionConfigBase config, IPresentationContext context)
        {
            this.Logger = LogHelper.GetLogger(this.GetType());
            this.Config = config;
            this.Context = context;

            if (this.Config.Enabled)
            {
                this.EnabledHandler = new DynamicPropertyHandler(this.Config.EnabledProperty, true, context);
            }
        }

        /// <summary>
        /// Event that is fired whenever this object's
        /// <see cref="IsEnabled"/> changes its value.
        /// </summary>
        public event EventHandler EnabledChanged;

        /// <summary>
        /// Event that is fired whenever this section has finished showing after
        /// the defined duration. The duration is measured from the moment when
        /// <see cref="ShowNextObject"/> (or <see cref="Activate"/>) is called.
        /// </summary>
        public event EventHandler DurationExpired;

        /// <summary>
        /// Gets the section configuration.
        /// </summary>
        public SectionConfigBase Config { get; private set; }

        /// <summary>
        /// Gets the currently shown object or null if none has been selected.
        /// </summary>
        public T CurrentObject { get; private set; }

        /// <summary>
        /// Gets the enabled handler.
        /// </summary>
        public DynamicPropertyHandler EnabledHandler { get; private set; }

        /// <summary>
        /// Gets the presentation context of this section.
        /// </summary>
        protected IPresentationContext Context { get; private set; }

        /// <summary>
        /// Check if this section is currently enabled.
        /// </summary>
        /// <returns>
        /// true if this section is enabled either by configuration or by evaluation.
        /// </returns>
        public virtual bool IsEnabled()
        {
            return this.EnabledHandler != null ? this.EnabledHandler.BoolValue : this.Config.Enabled;
        }

        /// <summary>
        /// Activates this section.
        /// </summary>
        /// <returns>true if we were able to find the layout for this section</returns>
        public bool Activate()
        {
            if (!this.IsEnabled() || !this.ShowNextObject())
            {
                return false;
            }

            if (this.EnabledHandler != null)
            {
                this.EnabledHandler.ValueChanged += this.EnabledHandlerOnValueChanged;
            }

            return true;
        }

        /// <summary>
        /// Deactivates this section.
        /// </summary>
        public virtual void Deactivate()
        {
            this.Context.Time.RemoveTimeElapsedHandler(this.currentDuration, this.OnDurationExpired);
            if (this.EnabledHandler != null)
            {
                this.EnabledHandler.ValueChanged -= this.EnabledHandlerOnValueChanged;
            }
        }

        /// <summary>
        /// Searches for the next page to show and sets
        /// <see cref="CurrentObject"/> if it was found.
        /// </summary>
        /// <returns>
        /// true if a new page was found.
        /// </returns>
        public virtual bool ShowNextObject()
        {
            if (!this.IsEnabled())
            {
                return false;
            }

            this.CurrentObject = this.FindNextObject();
            if (this.CurrentObject == null)
            {
                return false;
            }

            this.StartTimer();
            return true;
        }

        /// <summary>
        /// Gets the <see cref="LayoutBase"/> implementation of the currently
        /// valid layout (or null) from the this section.
        /// </summary>
        /// <returns>
        /// The <see cref="LayoutBase"/> or null if no layout can be shown.
        /// </returns>
        public abstract LayoutBase GetLayout();

        /// <summary>
        /// Prepares the given item to be shown for this section.
        /// Subclasses can use this method to modify the item before it is sent
        /// to the renderer.
        /// </summary>
        /// <param name="item">
        /// The screen item.
        /// </param>
        public virtual void PrepareItem(ScreenItemBase item)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            this.Deactivate();
            if (this.EnabledHandler != null)
            {
                this.EnabledHandler.Dispose();
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<string>("Layout", this.Config.Layout, true);
        }

        /// <summary>
        /// Finds the next available page.
        /// This method has to be implemented by different types of sections.
        /// </summary>
        /// <returns>
        /// The page to be shown or null if no page can be shown and the cycle should
        /// switch to the next section.
        /// </returns>
        protected abstract T FindNextObject();

        /// <summary>
        /// Gets the duration for this section.
        /// </summary>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        protected internal virtual TimeSpan GetDuration()
        {
            return this.Config.Duration;
        }

        /// <summary>
        /// Starts the timer that will elapse after <see cref="GetDuration"/>.
        /// </summary>
        protected virtual void StartTimer()
        {
            this.currentDuration = this.GetDuration();
            this.Context.Time.AddTimeElapsedHandler(this.currentDuration, this.OnDurationExpired);
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

        /// <summary>
        /// Raises the <see cref="DurationExpired"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseDurationExpired(EventArgs e)
        {
            var handler = this.DurationExpired;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void EnabledHandlerOnValueChanged(object sender, EventArgs e)
        {
            this.RaiseEnabledChanged(e);
        }

        private void OnDurationExpired(DateTime time)
        {
            this.RaiseDurationExpired(EventArgs.Empty);
        }
    }
}