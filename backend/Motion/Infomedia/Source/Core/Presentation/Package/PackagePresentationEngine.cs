// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagePresentationEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentationEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Package
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Motion.Infomedia.Core.Presentation.Composer;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The package presentation engine used to present a <see cref="CyclePackageConfig"/>
    /// for a certain virtual display (<see cref="VirtualDisplayConfig"/>).
    /// </summary>
    public class PackagePresentationEngine : PresentationEngineBase<Page>
    {
        private readonly VirtualDisplayConfig displayConfig;

        private readonly PagedPresentationContext pageContext;

        private readonly ICycleLoggingService cycleLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackagePresentationEngine"/> class.
        /// </summary>
        /// <param name="displayConfig">
        /// The display config.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        public PackagePresentationEngine(
            VirtualDisplayConfig displayConfig, IComposer parent, IPresentationContext context)
            : base(new Size(displayConfig.Width, displayConfig.Height), context, parent)
        {
            this.displayConfig = displayConfig;
            this.pageContext = new PagedPresentationContext(context);

            try
            {
                // This throws exception as the type used is an interface so we end up creating the NullCycleLogger below
                this.cycleLogger = ServiceLocator.Current.GetInstance<ICycleLoggingService>();
            }
            catch
            {
                // no logger was found, let's use the NullCycleLogger
                this.cycleLogger = new NullCycleLogger();
            }
        }

        /// <summary>
        /// Gets the package cycle manager.
        /// </summary>
        public PackageCycleManager PackageCycleManager { get; private set; }

        /// <summary>
        /// Does the actual implementation of <see cref="PresentationEngineBase{T}.Start"/>.
        /// </summary>
        protected override void DoStart()
        {
            this.BeginUpdate();
            base.DoStart();
            this.EndUpdate(new PresentationUpdatedEventArgs());
        }

        /// <summary>
        /// Creates the cycle manager to be used by this engine.
        /// </summary>
        /// <returns>
        /// A new <see cref="PackageCycleManager"/>.
        /// </returns>
        protected override CycleManagerBase<Page> CreateCycleManager()
        {
            var cyclePackage =
                this.Context.Config.Config.CyclePackages.Find(p => p.Name == this.displayConfig.CyclePackage);
            if (cyclePackage == null)
            {
                throw new KeyNotFoundException("Can't find cycle package with name " + this.displayConfig.CyclePackage);
            }

            this.PackageCycleManager = new PackageCycleManager(cyclePackage, this.Context);
            return this.PackageCycleManager;
        }

        /// <summary>
        /// Loads all layout elements for the given section.
        /// </summary>
        /// <param name="section">
        /// The section.
        /// </param>
        /// <returns>
        /// An enumeration over all layout elements including the ones from base layouts
        /// (see <see cref="LayoutConfig.BaseLayoutName"/>).
        /// </returns>
        protected override IEnumerable<ElementBase> LoadLayoutElements(SectionBase<Page> section)
        {
            this.pageContext.ChangePage(section.CurrentObject);

            return base.LoadLayoutElements(section);
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
            // use the page context, not the default context for all composers
            return ComposerFactory.CreateComposer(this.pageContext, parent, element);
        }

        /// <summary>
        /// Reload the currently shown page.
        /// </summary>
        /// <param name="page">
        /// The current page.
        /// </param>
        protected override void ReloadCurrentObject(Page page)
        {
            this.pageContext.ChangePage(page);
            base.ReloadCurrentObject(page);
            this.cycleLogger.PageChanged(this.CurrentCycle, this.Context);
        }

        /// <summary>
        /// Raises the <see cref="PresentationEngineBase{T}.CurrentRootChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments
        /// </param>
        protected override void RaiseCurrentRootChanged(EventArgs e)
        {
            this.cycleLogger.PageChanged(this.CurrentCycle, this.Context);
            base.RaiseCurrentRootChanged(e);
        }

        private class NullCycleLogger : ICycleLoggingService
        {
            public void PageChanged(CycleBase<Page> currentCycle, IPresentationContext context)
            {
            }
        }
    }
}
