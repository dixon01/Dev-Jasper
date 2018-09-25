// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediTreeInfoPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediTreeInfoPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.App
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Diag.Core.Resources;
    using Gorba.Center.Diag.Core.ViewModels.MediTree;

    /// <summary>
    /// The view model for the application part that shows the Medi management tree.
    /// </summary>
    public class MediTreeInfoPartViewModel : AppInfoPartViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        private MediTreeNodeViewModel selectedNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediTreeInfoPartViewModel"/> class.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public MediTreeInfoPartViewModel(RemoteAppViewModel application, ICommandRegistry commandRegistry)
            : base(application)
        {
            this.commandRegistry = commandRegistry;
            this.Name = DiagStrings.AppInfoPart_MediManagementTree;
        }

        /// <summary>
        /// Gets the load file system folder command
        /// </summary>
        public ICommand LoadFileSystemFolderCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.UnitApplication.LoadMediTreeChildren);
            }
        }

        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        public MediTreeNodeViewModel SelectedNode
        {
            get
            {
                return this.selectedNode;
            }

            set
            {
                this.SetProperty(ref this.selectedNode, value, () => this.SelectedNode);
            }
        }
    }
}