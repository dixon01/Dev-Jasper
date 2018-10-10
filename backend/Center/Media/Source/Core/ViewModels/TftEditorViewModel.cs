// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TftEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TftEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System.Windows.Input;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// the layout editor view model
    /// </summary>
    public class TftEditorViewModel : GraphicalEditorViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TftEditorViewModel"/> class.
        /// </summary>
        public TftEditorViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TftEditorViewModel"/> class
        /// </summary>
        /// <param name="parent">the parent view model</param>
        /// <param name="commandRegistry">the Command Registry</param>
        public TftEditorViewModel(IMediaShell parent, ICommandRegistry commandRegistry)
            : base(parent, commandRegistry)
        {
            this.LayoutRenderer = new PreviewLayoutRenderer(this, commandRegistry);
        }

        /// <summary>
        /// Gets the CreateLayoutElement Command.
        /// </summary>
        public override ICommand CreateLayoutElementCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.Tft.CreateElement);
            }
        }

        /// <summary>
        /// Gets the ShowElementEditPopup command.
        /// </summary>
        public override ICommand ShowElementEditPopupCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.Tft.ShowLayoutEditPopup);
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
                return SnapConfiguration.Tft;
            }
        }

        /// <summary>
        /// Gets the paste configuration.
        /// </summary>
        public override PasteConfiguration PasteConfiguration
        {
            get
            {
                return PasteConfiguration.Tft;
            }
        }
    }
}
