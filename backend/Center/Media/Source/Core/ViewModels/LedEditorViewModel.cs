// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The led editor view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The led editor view model.
    /// </summary>
    public class LedEditorViewModel : GraphicalEditorViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LedEditorViewModel"/> class
        /// </summary>
        /// <param name="parent">the parent view model</param>
        /// <param name="commandRegistry">the Command Registry</param>
        public LedEditorViewModel(IMediaShell parent, ICommandRegistry commandRegistry)
            : base(parent, commandRegistry)
        {
            this.LayoutRenderer = new LedPreviewRenderer(this, commandRegistry);
        }

        /// <summary>
        /// Gets the CreateLayoutElement Command.
        /// </summary>
        public override ICommand CreateLayoutElementCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.Led.CreateElement);
            }
        }

        /// <summary>
        /// Gets the ShowElementEditPopup command.
        /// </summary>
        public override ICommand ShowElementEditPopupCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.Led.ShowLayoutEditPopup);
            }
        }

        /// <summary>
        /// Gets the copy paste configuration.
        /// </summary>
        public override PasteConfiguration PasteConfiguration
        {
            get
            {
                return PasteConfiguration.Led;
            }
        }

        /// <summary>
        /// Gets the snap configuration.
        /// </summary>
        /// <remarks>Edge snap is disabled for the audio editor.</remarks>
        public override SnapConfiguration SnapConfiguration
        {
            get
            {
                return SnapConfiguration.Led;
            }
        }
    }
}
