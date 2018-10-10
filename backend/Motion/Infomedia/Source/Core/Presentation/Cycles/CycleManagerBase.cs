// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleManagerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Manager for cycles. It looks at the enabled dynamic properties and the cycle priorities
//   to decide which layout to show.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management;

    using NLog;

    /// <summary>
    /// Base class for all managers for cycles. It looks at the enabled dynamic properties and the cycle priorities
    /// to decide which section (and which object within that section) to show.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item inside the cycle's sections. To be defined by subclasses.
    /// </typeparam>
    public abstract class CycleManagerBase<T> : IDisposable, IManageableObject
        where T : class
    {
        private readonly IPresentationContext context;

        private readonly Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleManagerBase{T}"/> class.
        /// </summary>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        protected CycleManagerBase(IPresentationContext context)
        {
            this.EventCycleInfos = new List<EventCycleInfo>();
            this.StandardCycles = new List<CycleBase<T>>();
            this.context = context;
            this.logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets the current cycle or null if no cycle was selected.
        /// </summary>
        public CycleBase<T> CurrentCycle { get; private set; }

        /// <summary>
        /// Gets the standard cycles.
        /// </summary>
        public List<CycleBase<T>> StandardCycles { get; private set; }

        /// <summary>
        /// Gets the event cycle info.
        /// </summary>
        public List<EventCycleInfo> EventCycleInfos { get; private set; }

        /// <summary>
        /// Shows the next page of the current step in this cycle or the first
        /// page of the next cycle.
        /// This method will set <see cref="CurrentCycle"/>.
        /// </summary>
        public void ShowNextPage()
        {
            if (this.CurrentCycle == null || !this.CurrentCycle.ShowNextObject())
            {
                this.FindValidCycle();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var cycle in this.StandardCycles)
            {
                cycle.EnabledChanged -= this.CycleOnEnabledChanged;
                cycle.Dispose();
            }

            foreach (var info in this.EventCycleInfos)
            {
                info.Triggered -= this.EventCycleInfoOnTriggered;
                info.Cycle.EnabledChanged -= this.CycleOnEnabledChanged;
                info.Dispose();
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var standardCycle in this.StandardCycles)
            {
                yield return parent.Factory.CreateManagementProvider(standardCycle.Config.Name, parent, standardCycle);
            }

            foreach (var eventCycle in this.EventCycleInfos)
            {
                yield return parent.Factory.CreateManagementProvider(eventCycle.Cycle.Config.Name, parent, eventCycle);
            }
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            var cycle = this.CurrentCycle;
            if (cycle == null)
            {
                yield break;
            }

            yield return new ManagementProperty<string>("Current Cycle", cycle.Config.Name, true);

            if (cycle.CurrentSection == null)
            {
                yield break;
            }

            yield return new ManagementProperty<int>(
                "Current Section Duration", (int)cycle.CurrentSection.Config.Duration.TotalSeconds, true);
            yield return new ManagementProperty<string>("Current Layout", cycle.CurrentSection.Config.Layout, true);
        }

        /// <summary>
        /// Add a standard cycle to this manager.
        /// This has to be called from the subclass' constructor.
        /// </summary>
        /// <param name="cycle">
        /// The cycle.
        /// </param>
        protected void AddStandardCycle(CycleBase<T> cycle)
        {
            this.StandardCycles.Add(cycle);
            cycle.EnabledChanged += this.CycleOnEnabledChanged;
        }

        /// <summary>
        /// Add a standard cycle to this manager.
        /// This has to be called from the subclass' constructor.
        /// </summary>
        /// <param name="cycle">
        /// The event cycle.
        /// </param>
        protected void AddEventCycle(EventCycleBase<T> cycle)
        {
            var info = new EventCycleInfo(cycle);
            info.Triggered += this.EventCycleInfoOnTriggered;
            info.Cycle.EnabledChanged += this.CycleOnEnabledChanged;
            this.EventCycleInfos.Add(info);
        }

        private void EventCycleInfoOnTriggered(object sender, EventArgs eventArgs)
        {
            this.FindValidCycle();
        }

        /// <summary>
        /// Finds the first valid cycle and sets it as the current cycle,
        /// thus setting the current step and the current layout.
        /// </summary>
        private void FindValidCycle()
        {
            // find if any event cycle was triggered
            foreach (var info in this.EventCycleInfos)
            {
                if (!info.WasTriggered)
                {
                    // the event cycle was not triggered since it was last shown
                    continue;
                }

                if (info.Cycle == this.CurrentCycle
                    && (info.Cycle.CurrentSection == null || info.Cycle.CurrentSection.CurrentObject == null))
                {
                    // the current event cycle is invalid (no next page), so let's reset it later when we deactivate it
                    info.WasTriggered = false;
                    info.Cycle.ShouldReset = true;
                    continue;
                }

                if (info.Cycle.IsEnabled())
                {
                    if (this.SetCurrentCycle(info.Cycle))
                    {
                        return;
                    }
                }
            }

            foreach (var cycle in this.StandardCycles)
            {
                if (!cycle.IsEnabled())
                {
                    // the cycle has been disabled, so let's reset it later when we deactivate it
                    cycle.ShouldReset = true;
                    continue;
                }

                if (this.SetCurrentCycle(cycle))
                {
                    return;
                }
            }

            this.logger.Warn("Couldn't find a valid cycle, using a null cycle");
            this.SetCurrentCycle(new NullCycle<T>(this.context));
        }

        /// <summary>
        /// Sets the current cycle. Does nothing and returns true
        /// if the cycle is already the same.
        /// </summary>
        /// <param name="cycle">the new cycle to set</param>
        /// <returns>true if we were able to find a valid step for this cycle</returns>
        private bool SetCurrentCycle(CycleBase<T> cycle)
        {
            if (this.CurrentCycle == cycle)
            {
                return true;
            }

            this.logger.Info("Changing to cycle: {0}", cycle.Config.Name);

            if (!cycle.Activate())
            {
                return false;
            }

            if (this.CurrentCycle != null)
            {
                this.CurrentCycle.Deactivate();
            }

            this.CurrentCycle = cycle;
            return true;
        }

        private void CycleOnEnabledChanged(object sender, EventArgs e)
        {
            this.FindValidCycle();
        }

        /// <summary>
        /// The event cycle info.
        /// </summary>
        public class EventCycleInfo : IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EventCycleInfo"/> class.
            /// </summary>
            /// <param name="cycle">
            /// The event cycle.
            /// </param>
            public EventCycleInfo(EventCycleBase<T> cycle)
            {
                this.Cycle = cycle;
                this.Cycle.Triggered += this.EventCycleOnTriggered;
            }

            /// <summary>
            /// Event that is fired this cycle is enabled.
            /// </summary>
            public event EventHandler Triggered;

            /// <summary>
            /// Gets the cycle.
            /// </summary>
            public EventCycleBase<T> Cycle { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether cycle was triggered.
            /// </summary>
            public bool WasTriggered { get; set; }

            /// <summary>
            /// The dispose.
            /// </summary>
            public void Dispose()
            {
                this.Cycle.Triggered -= this.EventCycleOnTriggered;
                this.Cycle.Dispose();
            }

            private void EventCycleOnTriggered(object sender, EventArgs e)
            {
                this.WasTriggered = true;

                var handler = this.Triggered;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }
    }
}