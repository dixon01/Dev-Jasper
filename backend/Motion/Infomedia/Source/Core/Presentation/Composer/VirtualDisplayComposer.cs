// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualDisplayComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VirtualDisplayComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Motion.Infomedia.Core.Presentation.Package;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Presenter for a <see cref="VirtualDisplayRefConfig"/>.
    /// It creates an <see cref="IncludeItem"/>.
    /// It uses a <see cref="PackagePresentationEngine"/> to handle the presentation.
    /// </summary>
    public sealed class VirtualDisplayComposer : DrawableComposerBase<VirtualDisplayRefConfig, IncludeItem>, IManageable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualDisplayComposer"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="element">
        /// The presentation element.
        /// </param>
        public VirtualDisplayComposer(
            IPresentationContext context, IComposer parent, VirtualDisplayRefConfig element)
            : base(context, parent, element)
        {
            this.VirtualDisplay = context.Config.Config.VirtualDisplays.Find(d => d.Name == element.Reference);
            if (this.VirtualDisplay == null)
            {
                throw new KeyNotFoundException("Couldn't find virtual display " + element.Reference);
            }

            this.InitializeItem();

            this.PresentationEngine = new PackagePresentationEngine(this.VirtualDisplay, this, context);
            this.PresentationEngine.CurrentRootChanged += this.PresentationEngineOnCurrentRootChanged;
            this.PresentationEngine.Start();
        }

        /// <summary>
        /// Gets the virtual display.
        /// </summary>
        public VirtualDisplayConfig VirtualDisplay { get; private set; }

        /// <summary>
        /// Gets the presentation engine.
        /// </summary>
        public PackagePresentationEngine PresentationEngine { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            this.PresentationEngine.Stop();
            this.PresentationEngine.CurrentRootChanged -= this.PresentationEngineOnCurrentRootChanged;
            this.PresentationEngine.Dispose();
            base.Dispose();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            var manageable = this.PresentationEngine as IManageable;
            return manageable == null ? new IManagementProvider[0] : manageable.GetChildren(parent);
        }

        private void PresentationEngineOnCurrentRootChanged(object sender, EventArgs eventArgs)
        {
            this.Item.Include = this.PresentationEngine.CurrentRoot;
        }
    }
}