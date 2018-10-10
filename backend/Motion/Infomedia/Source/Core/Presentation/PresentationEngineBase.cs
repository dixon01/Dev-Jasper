// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationEngineBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Engine that does the layout and cycle handling using s.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Motion.Infomedia.Core.Presentation.Composer;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using NLog;

    /// <summary>
    /// Base class for the engines that do the layout and cycle handling using <see cref="IComposer"/>s.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item inside the cycle's sections. To be defined by subclasses.
    /// </typeparam>
    public abstract class PresentationEngineBase<T> : IManageable, IDisposable
        where T : class
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly Size size;

        private CycleManagerBase<T> cycleManager;

        private bool running;

        private bool preventItemUpdates;

        private ScreenUpdate currentUpdate;

        private SectionBase<T> lastSection;

        private string lastLayoutName;

        private bool updating;

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationEngineBase{T}"/> class.
        /// </summary>
        /// <param name="size">
        /// The size of the presentation.
        /// </param>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        /// <param name="parent">
        /// The parent composer for all composers created in this engine. Can be null.
        /// </param>
        protected PresentationEngineBase(Size size, IPresentationContext context, IComposer parent)
        {
            this.Composers = new List<IComposer>();
            this.Logger = LogManager.GetLogger(this.GetType().FullName);

            this.size = size;
            this.Context = context;
            this.ParentComposer = parent;
        }

        /// <summary>
        /// Event that is fired whenever the <see cref="CurrentRoot"/> changes.
        /// </summary>
        public event EventHandler CurrentRootChanged;

        /// <summary>
        /// Gets the currently displayed root item of this engine.
        /// Whenever this property changes, the <see cref="CurrentRootChanged"/>
        /// event is fired.
        /// </summary>
        public RootItem CurrentRoot { get; private set; }

        /// <summary>
        /// Gets the parent <see cref="IComposer"/>.
        /// This value is set so nested presentation engines can inherit
        /// properties (like visibility and coordinates/size).
        /// </summary>
        public IComposer ParentComposer { get; private set; }

        /// <summary>
        /// Gets the list of composers for the layout.
        /// </summary>
        public List<IComposer> Composers { get; private set; }

        /// <summary>
        /// Gets the presentation context.
        /// </summary>
        protected IPresentationContext Context { get; private set; }

        /// <summary>
        /// Gets the current cycle from the cycle manager.
        /// </summary>
        protected CycleBase<T> CurrentCycle
        {
            get
            {
                return this.cycleManager.CurrentCycle;
            }
        }

        /// <summary>
        /// Starts this engine by querying the cycle manager for a first time.
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            if (this.cycleManager == null)
            {
                this.cycleManager = this.CreateCycleManager();
            }

            this.DoStart();
        }

        /// <summary>
        /// Stops this engine.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;

            this.Context.Updating -= this.ContextOnUpdating;
            this.Context.Updated -= this.ContextOnUpdated;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.Stop();
            this.cycleManager.Dispose();

            this.DisposeComposers(this.Composers);
            this.Composers.Clear();

            this.cycleManager.Dispose();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider("Cycles", parent, this.cycleManager);

            yield return parent.Factory.CreateManagementProvider("Current Layout", parent, this.CurrentRoot);

            yield return
                parent.Factory.CreateManagementProvider(
                    "Nested Presentations", parent, new ManageableComposerList(this.Composers));
        }

        /// <summary>
        /// Does the actual implementation of <see cref="Start"/>.
        /// This can be overridden by subclasses if they need to do something when starting.
        /// </summary>
        protected virtual void DoStart()
        {
            this.Context.Updating += this.ContextOnUpdating;
            this.Context.Updated += this.ContextOnUpdated;

            this.ShowNextObject();
        }

        /// <summary>
        /// Begins the process of updating this presentation.
        /// Subclasses can override this method to be informed when they are being updated.
        /// </summary>
        protected virtual void BeginUpdate()
        {            
            if (this.updating)
            {
                throw new NotSupportedException("Can't call BeginUpdate() twice without EndUpdate() in between");
            }
            
            this.updating = true;

            var oldCycle = this.cycleManager.CurrentCycle;
            if (oldCycle == null)
            {
                this.lastSection = null;
                this.lastLayoutName = null;
            }
            else
            {
                this.lastSection = oldCycle.CurrentSection;
                this.lastLayoutName = this.GetLayoutName(oldCycle.CurrentSection);
            }
            
            if (this.CurrentRoot != null)
            {
                this.currentUpdate = new ScreenUpdate { RootId = this.CurrentRoot.Id };
                this.Logger.Info("ScreenUpdate RootId={0}", this.currentUpdate.RootId);
            }
        }

        /// <summary>
        /// Finishes the process of updating this presentation.
        /// Subclasses can override this method to be informed when they were updated.
        /// </summary>
        /// <param name="e">
        /// The event arguments to which the class can add updates.
        /// </param>
        protected virtual void EndUpdate(PresentationUpdatedEventArgs e)
        {            
            if (!this.updating)
            {
                throw new NotSupportedException("Can't call EndUpdate() before BeginUpdate()");
            }

            this.updating = false;

            var update = this.currentUpdate;
            this.currentUpdate = null;

            if (this.lastSection != null)
            {
                this.lastSection.DurationExpired -= this.CurrentSectionOnDurationExpired;
            }

            var currentSection = this.cycleManager.CurrentCycle.CurrentSection;
            if (currentSection != null)
            {
                currentSection.DurationExpired += this.CurrentSectionOnDurationExpired;
            }

            if (update == null || this.GetLayoutName(currentSection) != this.lastLayoutName)
            {
                // load the new layout (this will raise the CurrentRootChanged event)
                if (currentSection != null)
                {
                    this.LoadLayout(currentSection);
                }
                else
                {
                    var currentCycleName = this.cycleManager.CurrentCycle?.Config.Name;
                    this.Logger.Error("currentSection = null CurrentCycle does not have a current section Name = {0}", currentCycleName);
                }
            }
            else if (update.Updates.Count > 0)
            {
                try
                {
                    // send the update to the old screen
                    this.Logger.Debug("Updating {1} screen items on #{0}", update.RootId, update.Updates.Count);

                    if (this.Logger.IsTraceEnabled)
                    {
                        foreach (var itemUpdate in update.Updates)
                        {
                            this.Logger.Trace("- #{0}: {1}={2}", itemUpdate.ScreenItemId, itemUpdate.Property, itemUpdate.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.Error("EndUpdate Exception {0}", ex.Message);
                }

                e.Updates.Add(update);
            }
        }

        /// <summary>
        /// Creates the cycle manager to be used by this engine.
        /// Subclasses have to implement this method to return a specific implementation
        /// of <see cref="CycleManagerBase{T}"/> for the type <see cref="T"/>.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="CycleManagerBase{T}"/>.
        /// </returns>
        protected abstract CycleManagerBase<T> CreateCycleManager();

        /// <summary>
        /// Loads all layout elements for the given section.
        /// Subclasses need to query the section object to figure out which
        /// layout elements should be shown.
        /// </summary>
        /// <param name="section">
        /// The section.
        /// </param>
        /// <returns>
        /// An enumeration over all layout elements (<see cref="ElementBase"/> subclasses).
        /// </returns>
        protected virtual IEnumerable<ElementBase> LoadLayoutElements(SectionBase<T> section)
        {
            var layout = section.GetLayout();
            if (layout == null)
            {
                this.Logger.Warn("No layout to load!");
                return new ElementBase[0];
            }

            this.Logger.Info("Loading layout {0}", layout.Name);
            return layout.LoadLayoutElements(this.size.Width, this.size.Height);
        }

        /// <summary>
        /// Creates the composer for the given element.
        /// </summary>
        /// <param name="element">
        /// The element for which to create a composer.
        /// </param>
        /// <param name="parent">
        /// The parent composer for nested elements.
        /// </param>
        /// <returns>
        /// The <see cref="IComposer"/>.
        /// </returns>
        protected abstract IComposer CreateComposer(ElementBase element, IComposer parent);

        /// <summary>
        /// Reload the currently shown object.
        /// </summary>
        /// <param name="currentObject">
        /// The current object.
        /// </param>
        protected virtual void ReloadCurrentObject(T currentObject)
        {
        }

        /// <summary>
        /// Sends the property change of an <see cref="ItemBase"/> to the renderer.
        /// </summary>
        /// <param name="item">
        /// The item of which a property changed.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The new value of the property.
        /// </param>
        /// <param name="animation">
        /// The animation.
        /// </param>
        protected void SendItemPropertyChange(
            ItemBase item, string propertyName, object value, PropertyChangeAnimation animation)
        {
            if (this.preventItemUpdates)
            {
                return;
            }

            var update = new ItemUpdate
                             {
                                 ScreenItemId = item.Id,
                                 Property = propertyName,
                                 Value = value,
                                 Animation = animation
                             };
            this.Logger.Info("SendItemPropertyChange() {0}", update.ToString());
            if (value is ImageItem)
            {
                this.Logger.Info("ImageItem Updated");
            }
            if (this.currentUpdate == null)
            {
                this.Logger.Warn("Updated item #{0} outside update process", item.Id);
                return;
            }

            this.currentUpdate.Updates.Add(update);
        }

        /// <summary>
        /// Raises the <see cref="CurrentRootChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments
        /// </param>
        protected virtual void RaiseCurrentRootChanged(EventArgs e)
        {
            var handler = this.CurrentRootChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void LoadLayout(SectionBase<T> section)
        {
            try
            {
                var duration = section.GetDuration();
                var layoutName = section.GetLayout().Name;
                this.preventItemUpdates = true;
                this.Logger.Info("Loading layout {0}", section.Config.Layout);
                var newComposers = new List<IComposer>();
                List<string> resourcePath = new List<string>();
                foreach (var element in this.LoadLayoutElements(section))
                {
                    this.CreateComposers(element, this.ParentComposer, newComposers);
                }
                List<ScreenItemBase> updates = new List<ScreenItemBase>();
                
                var root = new RootItem { Width = this.size.Width, Height = this.size.Height };
                this.Logger.Info("RootItem {0}", root.ToString());
                foreach (var composer in newComposers)
                {
                    var presentableComposer = composer as IPresentableComposer;
                    if (presentableComposer != null)
                    {
                        presentableComposer.ItemPropertyValueChanged += this.OnItemPropertyValueChanged;
                        var composerItem = presentableComposer.Item;
                        section.PrepareItem(composerItem);
                        root.Items.Add(composerItem);

                        ImageItem imageItem = composerItem as ImageItem;
                        if (imageItem != null)
                        {
                            updates.Add(imageItem);
                            continue;
                        }

                        VideoItem videoItem = composerItem as VideoItem;
                        if (videoItem != null)
                        {
                            updates.Add(videoItem);
                        }
                    }
                }
                
                var oldComposers = this.Composers;

                this.CurrentRoot = root;
                this.Composers = newComposers;
                MessageDispatcher.Instance.Broadcast(new UnitsFeedBackMessage<string>($"{layoutName} "));
                this.DisposeComposers(oldComposers);
                this.RaiseCurrentRootChanged(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                this.Logger.Error("LoadLayout Exception {0}", ex.Message);
                //  throw;
            }
            finally
            {
                this.preventItemUpdates = false;
            }
        }

        private void CreateComposers(ElementBase element, IComposer parent, ICollection<IComposer> newComposers)
        {
            try
            {
                var composer = this.CreateComposer(element, parent);
                newComposers.Add(composer);

                var composite = element as ICompositeElement;
                if (composite == null)
                {
                    return;
                }

                foreach (var childElement in composite)
                {
                    this.CreateComposers(childElement, composer, newComposers);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't create composer from element");
            }
        }

        private void DisposeComposers(IEnumerable<IComposer> oldComposers)
        {
            foreach (var composer in oldComposers)
            {
                var presentableComposer = composer as IPresentableComposer;
                if (presentableComposer != null)
                {
                    presentableComposer.ItemPropertyValueChanged -= this.OnItemPropertyValueChanged;
                }

                composer.Dispose();
            }
        }

        private void CurrentSectionOnDurationExpired(object sender, EventArgs eventArgs)
        {
            if (!this.running)
            {
                return;
            }

            this.Logger.Info("CurrentSectionOnDurationExpired() Event Calling ShowNextObject");
            this.ShowNextObject();
        }

        private void ShowNextObject()
        {
            this.cycleManager.ShowNextPage();

            var currentSection = this.cycleManager.CurrentCycle.CurrentSection;
            var secondsDuration = currentSection.Config?.Duration.TotalSeconds;
            var layoutName = this.GetLayoutName(currentSection);
            this.Logger.Info("ShowNextObject cycleManager.ShowNextPage() layoutName = {0} Duration Seconds = {1}", layoutName, secondsDuration);

            if (layoutName == this.lastLayoutName)
            {
                this.ReloadCurrentObject(currentSection.CurrentObject);
            }
        }

        private string GetLayoutName(SectionBase<T> section)
        {
            if (section == null)
            {
                return null;
            }

            var layout = section.GetLayout();
            return layout == null ? null : layout.Name;
        }

        private void ContextOnUpdating(object sender, EventArgs e)
        {
            this.BeginUpdate();
        }

        private void ContextOnUpdated(object sender, PresentationUpdatedEventArgs e)
        {
            this.EndUpdate(e);
        }

        private void OnItemPropertyValueChanged(object sender, AnimatedItemPropertyChangedEventArgs e)
        {
            this.SendItemPropertyChange(e.Item, e.PropertyName, e.Value, e.Animation);
        }

        private class ManageableComposerList : IManageable
        {
            private readonly List<IComposer> composers;

            public ManageableComposerList(List<IComposer> composers)
            {
                this.composers = composers;
            }

            public IEnumerable<IManagementProvider> GetChildren(IManagementProvider parent)
            {
                var index = 0;
                foreach (var composer in this.composers)
                {
                    var manageable = composer as IManageable;
                    if (manageable == null)
                    {
                        continue;
                    }

                    yield return
                        parent.Factory.CreateManagementProvider(
                            composer.GetType().Name + " " + (index++), parent, manageable);
                }
            }
        }
    }
}