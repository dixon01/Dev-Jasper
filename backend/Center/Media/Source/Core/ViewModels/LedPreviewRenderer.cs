// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedPreviewRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The led preview renderer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.Properties;

    /// <summary>
    /// The led preview renderer.
    /// </summary>
    public class LedPreviewRenderer : ILayoutRenderer
    {
         private readonly LedEditorViewModel parent;

        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="LedPreviewRenderer"/> class
        /// </summary>
        /// <param name="parentViewModel">the parent view model</param>
        /// <param name="commandRegistry">the Command Registry</param>
        public LedPreviewRenderer(LedEditorViewModel parentViewModel, ICommandRegistry commandRegistry)
        {
            this.parent = parentViewModel;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets the parent ViewModel
        /// </summary>
        public LedEditorViewModel Parent
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>
        /// Gets the radius of an LED dot.
        /// </summary>
        public int LedDotRadius
        {
            get
            {
                return Settings.Default.LedDotRadius;
            }
        }

        /// <summary>
        /// Gets the space between two LED dots.
        /// </summary>
        public int LedDotSpace
        {
            get
            {
                return Settings.Default.LedDotSpace;
            }
        }
    }
}
