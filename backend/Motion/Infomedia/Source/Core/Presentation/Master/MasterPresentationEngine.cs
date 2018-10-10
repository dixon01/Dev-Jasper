// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterPresentationEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterPresentationEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Master
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Motion.Infomedia.Core.Presentation.Composer;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Master;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// The master presentation engine used to present <see cref="MasterPresentationConfig"/>
    /// for a certain physical screen (<see cref="PhysicalScreenConfig"/>).
    /// </summary>
    public class MasterPresentationEngine : PresentationEngineBase<MasterLayout>, IManageableObject
    {
        private readonly SubPresentationContext subContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPresentationEngine"/> class.
        /// </summary>
        /// <param name="screenConfig">
        /// The screen config.
        /// </param>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        public MasterPresentationEngine(PhysicalScreenConfig screenConfig, IPresentationContext context)
            : base(new Size(screenConfig.Width, screenConfig.Height), context, null)
        {
            this.subContext = new SubPresentationContext(context);
            this.ScreenConfig = screenConfig;

            this.VisibleHandler = new DynamicPropertyHandler(screenConfig.VisibleProperty, true, context);
            this.VisibleHandler.ValueChanged += this.VisibleHandlerOnValueChanged;
        }

        /// <summary>
        /// Gets the physical screen config of this engine.
        /// </summary>
        public PhysicalScreenConfig ScreenConfig { get; private set; }

        /// <summary>
        /// Gets the current screen root which contains the <see cref="PresentationEngineBase{T}.CurrentRoot"/>.
        /// </summary>
        public ScreenRoot CurrentScreen { get; private set; }

        /// <summary>
        /// Gets the master cycle manager.
        /// </summary>
        public MasterCycleManager MasterCyleManager { get; private set; }

        /// <summary>
        /// Gets the virtual display composer.
        /// </summary>
        public VirtualDisplayComposer VirtualDisplayComposer { get; private set; }

        /// <summary>
        /// Gets the visible handler.
        /// </summary>
        public DynamicPropertyHandler VisibleHandler { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            this.VisibleHandler.ValueChanged -= this.VisibleHandlerOnValueChanged;
            base.Dispose();
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<string>("Type", this.ScreenConfig.Type.ToString(), true);
            yield return new ManagementProperty<string>("Id", this.ScreenConfig.Identifier, true);
            yield return new ManagementProperty<int>("Width", this.ScreenConfig.Width, true);
            yield return new ManagementProperty<int>("Height", this.ScreenConfig.Height, true);
        }

        /// <summary>
        /// Creates the cycle manager to be used by this engine.
        /// </summary>
        /// <returns>
        /// A new <see cref="MasterCycleManager"/>.
        /// </returns>
        protected override CycleManagerBase<MasterLayout> CreateCycleManager()
        {
            this.MasterCyleManager = new MasterCycleManager(this.ScreenConfig, this.Context);
            return this.MasterCyleManager;
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
        protected override IComposer CreateComposer(ElementBase element, IComposer parent)
        {
            var virtualDisplayRef = (VirtualDisplayRefConfig)element;
            this.VirtualDisplayComposer = new VirtualDisplayComposer(this.subContext, parent, virtualDisplayRef);
            return this.VirtualDisplayComposer;
        }

        /// <summary>
        /// Does the actual implementation of <see cref="PresentationEngineBase{T}.Start"/>.
        /// </summary>
        protected override void DoStart()
        {
            // we need to call BeginUpdate() here since after starting, EndUpdate() will get called
            this.BeginUpdate();
            base.DoStart();
        }

        /// <summary>
        /// Begins the process of updating this presentation.
        /// </summary>
        protected override void BeginUpdate()
        {
            // the order is very important: first we start updating ourselves, then we send the event to our children
            base.BeginUpdate();
            this.subContext.RaiseUpdating(EventArgs.Empty);
        }

        /// <summary>
        /// Finishes the process of updating this presentation.
        /// </summary>
        /// <param name="e">
        /// The event arguments to which the class can add updates.
        /// </param>
        protected override void EndUpdate(PresentationUpdatedEventArgs e)
        {
            // the order is very important: first we send the event to our children, then we finish the update ourselves
            this.subContext.RaiseUpdated(e);
            base.EndUpdate(e);
        }

        /// <summary>
        /// Raises the <see cref="PresentationEngineBase{T}.CurrentRootChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments
        /// </param>
        protected override void RaiseCurrentRootChanged(EventArgs e)
        {
            if (this.CurrentScreen != null)
            {
                this.CurrentScreen.PropertyValueChanged -= this.CurrentScreenOnPropertyValueChanged;
            }

            this.CurrentScreen = new ScreenRoot { Root = this.CurrentRoot, Visible = this.VisibleHandler.BoolValue };
            this.CurrentScreen.PropertyValueChanged += this.CurrentScreenOnPropertyValueChanged;
            base.RaiseCurrentRootChanged(e);
        }

        private void VisibleHandlerOnValueChanged(object sender, EventArgs e)
        {
            if (this.CurrentScreen != null)
            {
                this.CurrentScreen.Visible = this.VisibleHandler.BoolValue;
            }
        }

        private void CurrentScreenOnPropertyValueChanged(object sender, AnimatedPropertyChangedEventArgs e)
        {
            this.SendItemPropertyChange((ScreenRoot)sender, e.PropertyName, e.Value, e.Animation);
        }

        private class SubPresentationContext : IPresentationContext
        {
            private readonly IPresentationContext parent;

            public SubPresentationContext(IPresentationContext parent)
            {
                this.parent = parent;
            }

            public event EventHandler Updating;

            public event EventHandler<PresentationUpdatedEventArgs> Updated;

            public IPresentationConfigContext Config
            {
                get
                {
                    return this.parent.Config;
                }
            }

            public IPresentationGenericContext Generic
            {
                get
                {
                    return this.parent.Generic;
                }
            }

            public IPresentationTimeContext Time
            {
                get
                {
                    return this.parent.Time;
                }
            }

            public void RaiseUpdating(EventArgs e)
            {
                var handler = this.Updating;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            public void RaiseUpdated(PresentationUpdatedEventArgs e)
            {
                var handler = this.Updated;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }
    }
}