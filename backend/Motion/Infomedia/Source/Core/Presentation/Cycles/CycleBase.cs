// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleBase.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CycleBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    using NLog;

    /// <summary>
    /// Base class for cycles.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item inside this cycle's sections. To be defined by subclasses.
    /// </typeparam>
    public abstract class CycleBase<T> : IDisposable, IManageableObject
        where T : class
    {
        /// <summary>
        /// The logger used for logging information.
        /// </summary>
        protected readonly Logger Logger;

        private SectionBase<T> currentSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleBase{T}"/> class.
        /// </summary>
        /// <param name="config">
        /// The cycle config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="factory">
        /// The section factory, to be provided by the subclass.
        /// </param>
        protected CycleBase(CycleConfigBase config, IPresentationContext context, ISectionFactory<T> factory)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);

            this.Sections = new List<SectionBase<T>>();
            this.Config = config;

            this.EnabledHandler = new DynamicPropertyHandler(config.EnabledProperty, true, context);
            this.EnabledHandler.ValueChanged += this.EnabledHandlerOnValueChanged;

            foreach (var section in config.Sections)
            {
                try
                {
                    this.Sections.Add(factory.Create(section, context));
                }
                catch (Exception ex)
                {
                    this.Logger.Warn("Couldn't add section for " + section);
                    this.Logger.Debug(ex,"Couldn't add section for " + section);
                }
            }

            bool hasOneEnabledSection = false;
            foreach (var section in this.Sections)
            {
                if (section.IsEnabled())
                {
                    hasOneEnabledSection = true;
                    break;
                }
            }

            if (!hasOneEnabledSection)
            {
                this.Logger.Warn(
                    "There is no enabled section for cycle '{0}' . Please refer to the workaround in TD.",
                    this.Config.Name);
            }
        }

        /// <summary>
        /// Event that is fired every time <see cref="IsEnabled"/> changes its value.
        /// </summary>
        public event EventHandler EnabledChanged;

        /// <summary>
        /// Gets all the sections in this cycle.
        /// </summary>
        public List<SectionBase<T>> Sections { get; private set; }

        /// <summary>
        /// Gets the cycle configuration.
        /// </summary>
        public CycleConfigBase Config { get; private set; }

        /// <summary>
        /// Gets the currently shown step or null if no section is selected.
        /// </summary>
        public SectionBase<T> CurrentSection
        {
            get
            {
                return this.currentSection;
            }

            private set
            {
                if (this.currentSection != null)
                {
                    this.currentSection.EnabledChanged -= this.CurrentSectionOnEnabledChanged;
                }

                this.currentSection = value;
                if (this.currentSection != null)
                {
                    this.currentSection.EnabledChanged += this.CurrentSectionOnEnabledChanged;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cycle must be reset or not.
        /// </summary>
        public bool ShouldReset { get; set; }

        /// <summary>
        /// Gets the enabled handler.
        /// </summary>
        public DynamicPropertyHandler EnabledHandler { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is active.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets a value indicating whether should loop.
        /// </summary>
        protected virtual bool ShouldLoop
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Check if this cycle is currently valid.
        /// </summary>
        /// <returns>
        /// true if this cycle has no criteria or its criteria is valid and
        /// at least one step is also valid.
        /// </returns>
        public virtual bool IsEnabled()
        {
            if (!this.Config.Enabled)
            {
                return false;
            }

            if (this.EnabledHandler != null && !this.EnabledHandler.BoolValue)
            {
                return false;
            }

            var enabledSections = 0;
            foreach (var step in this.Sections)
            {
                if (step.IsEnabled())
                {
                    enabledSections++;
                }
            }

            if (enabledSections == 0)
            {
                this.Logger.Warn("Cycle '{0}' with {1} sections has no enabled section", this.Config.Name, this.Sections.Count);
            }

            return enabledSections > 0;
        }

        /// <summary>
        /// Tries to activate this cycle.
        /// </summary>
        /// <returns>
        /// True if the cycle was activated.
        /// </returns>
        public bool Activate()
        {
            this.IsActive = this.IsEnabled() && this.ShowNextObject();
            return this.IsActive;
        }

        /// <summary>
        /// Deactivates this cycle.
        /// </summary>
        public void Deactivate()
        {
            this.IsActive = false;
            if (this.CurrentSection != null)
            {
                this.CurrentSection.Deactivate();
            }

            if (this.ShouldReset)
            {
                this.Reset();
                this.ShouldReset = false;
            }
        }

        /// <summary>
        /// Switch to the next page in the current step of this cycle
        /// or to the next step of this cycle.
        /// </summary>
        /// <returns>
        /// If for some reason this cycle doesn't
        /// contain any valid step, false is returned.
        /// </returns>
        public bool ShowNextObject()
        {
            if (this.CurrentSection != null && this.CurrentSection.IsEnabled() && this.CurrentSection.ShowNextObject())
            {
                return true;
            }

            int index = this.Sections.IndexOf(this.CurrentSection);

            var stepCount = this.Sections.Count;
            for (int i = 0; i < stepCount; i++)
            {
                var oldSection = this.CurrentSection;
                index++;
                if (index >= stepCount)
                {
                    if (!this.ShouldLoop)
                    {
                        this.CurrentSection = null;
                        if (oldSection != null)
                        {
                            oldSection.Deactivate();
                        }

                        return false;
                    }

                    index = 0;
                }

                var section = this.Sections[index];
                var activated = section.Activate();
                if (!section.IsEnabled() || !activated)
                {
                    continue;
                }

                this.CurrentSection = section;
                if (oldSection != null && oldSection != section)
                {
                    oldSection.Deactivate();
                }

                this.Logger.Info("Changed to section #{0} of cycle '{1}'", index, this.Config.Name);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            this.Deactivate();
            this.Reset();

            foreach (var section in this.Sections)
            {
                section.Dispose();
            }

            this.EnabledHandler.ValueChanged -= this.EnabledHandlerOnValueChanged;
            this.EnabledHandler.Dispose();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            int index = 0;
            foreach (var section in this.Sections)
            {
                yield return
                    parent.Factory.CreateManagementProvider(string.Format("Section {0}", index), parent, section);
                index++;
            }
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<string>(
                "Current section duration", this.CurrentSection.Config.Duration.ToString(), true);
            yield return new ManagementProperty<string>(
                "Current section layout", this.CurrentSection.Config.Layout, true);
        }

        /// <summary>
        /// Resets the cycle.
        /// </summary>
        protected virtual void Reset()
        {
            // set the current step to null,
            // so next time we start again with the first step
            this.CurrentSection = null;
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

        private void EnabledHandlerOnValueChanged(object s, EventArgs e)
        {
            this.RaiseEnabledChanged(e);
        }

        private void CurrentSectionOnEnabledChanged(object sender, EventArgs eventArgs)
        {
            string x = "(";
            for (int i = 0; i < this.Sections.Count; i++)
            {
                x += i + "-" + this.Sections[i].Config.Layout
                    + "=" + this.Sections[i].IsEnabled() + ", ";
            }

            x += ")";
            this.Logger.Trace(x);

            if (!this.IsActive)
            {
                return;
            }

            /* always switch to the next section
            if (this.CurrentSection is StandardSection)
            {
                return;
            }*/

            if (!this.CurrentSection.IsEnabled())
            {
                this.ShowNextObject();
            }
        }
    }
}