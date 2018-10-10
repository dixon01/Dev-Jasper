// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreviewLayoutRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PreviewLayoutRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The PreviewLayoutRenderer.
    /// </summary>
    public class PreviewLayoutRenderer : ILayoutRenderer
    {
        private readonly TftEditorViewModel parent;

        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewLayoutRenderer"/> class
        /// </summary>
        /// <param name="parentViewModel">the parent view model</param>
        /// <param name="commandRegistry">the Command Registry</param>
        public PreviewLayoutRenderer(TftEditorViewModel parentViewModel, ICommandRegistry commandRegistry)
        {
            this.parent = parentViewModel;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets the parent ViewModel
        /// </summary>
        public EditorViewModelBase Parent
        {
            get
            {
                return this.parent;
            }
        }
    }
}