// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TftEditorToolbarViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TftEditorToolbarViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The view model representing the login information shown in the upper right corner.
    /// </summary>
    public class TftEditorToolbarViewModel : ViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        private EditorToolType liveInformationSelectedItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="TftEditorToolbarViewModel"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public TftEditorToolbarViewModel(MediaShell shell, ICommandRegistry commandRegistry)
        {
            this.Shell = shell;
            this.commandRegistry = commandRegistry;

            this.PropertyChanged += this.OnPropertyChanged;

            this.LiveInformationSelectedItem = EditorToolType.RssTicker;
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public MediaShell Shell { get; private set; }

        /// <summary>
        /// Gets or sets the live information selected item.
        /// </summary>
        public EditorToolType LiveInformationSelectedItem
        {
            get
            {
                return this.liveInformationSelectedItem;
            }

            set
            {
                this.SetProperty(ref this.liveInformationSelectedItem, value, () => this.LiveInformationSelectedItem);
            }
        }

        /// <summary>
        /// The selected editor tool changed. Called by the shell to update parent tool if new tool is a nested one.
        /// </summary>
        /// <param name="tool">
        /// The tool.
        /// </param>
        public void SelectedEditorToolChanged(EditorToolType tool)
        {
            if (tool == EditorToolType.RssTicker || tool == EditorToolType.LiveStream)
            {
                this.LiveInformationSelectedItem = tool;
            }
        }

        /// <summary>
        /// The set selected editor tool.
        /// </summary>
        public void SetSelectedEditorTool()
        {
            this.Shell.SelectedEditorTool = this.LiveInformationSelectedItem;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("LiveInformationSelectedItem"))
            {
                this.SetSelectedEditorTool();
            }
        }
    }
}