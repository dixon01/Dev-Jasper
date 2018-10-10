// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantSelectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The tenant selection view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Windows.Media;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The tenant selection view model.
    /// </summary>
    public class TenantSelectionViewModel : DialogViewModelBase, IStartupDialogViewModel
    {
        private readonly ICommandRegistry commandRegistry;

        private TenantReadableModel selectedTenant;

        private ActivityMessage activityMessage = new ActivityMessage();

        private bool isBusy;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantSelectionViewModel"/> class.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public TenantSelectionViewModel(IDialogFactory factory, ICommandRegistry commandRegistry)
            : base(factory)
        {
            this.commandRegistry = commandRegistry;
            this.WindowTitle = string.Empty;
        }

        /// <summary>
        /// Gets the state of the offline application.
        /// </summary>
        /// <value>
        /// The state of the offline application.
        /// </value>
        public IConnectedApplicationState ApplicationState
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IConnectedApplicationState>();
            }
        }

        /// <summary>
        /// Gets the tenants.
        /// </summary>
        public ObservableCollection<TenantReadableModel> Tenants
        {
            get
            {
                return this.ApplicationState.AuthorizedTenants;
            }
        }

        /// <summary>
        /// Gets or sets the selected tenant.
        /// </summary>
        /// <value>
        /// The selected tenant.
        /// </value>
        public TenantReadableModel SelectedTenant
        {
            get
            {
                return this.selectedTenant;
            }

            set
            {
                this.SetProperty(ref this.selectedTenant, value, () => this.SelectedTenant);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                if (this.SetProperty(ref this.isBusy, value, () => this.IsBusy))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(ClientCommandCompositionKeys.CancelTenantSelection);
            }
        }

        /// <summary>
        /// Gets the select command.
        /// </summary>
        public ICommand SelectTenantCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(ClientCommandCompositionKeys.TenantSelection);
            }
        }

        /// <summary>
        /// Gets or sets the activity message.
        /// </summary>
        /// <value>
        /// The activity message.
        /// </value>
        public ActivityMessage ActivityMessage
        {
            get
            {
                return this.activityMessage;
            }

            set
            {
                this.SetProperty(ref this.activityMessage, value, () => this.ActivityMessage);
            }
        }

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// Gets or sets the application title that is displayed in the header bar section.
        /// </summary>
        public string ApplicationTitle { get; set; }

        /// <summary>
        /// Gets or sets the application icon that is displayed in the header bar section.
        /// </summary>
        public ImageSource ApplicationIcon { get; set; }

        /// <summary>
        /// Gets or sets the version of this application.
        /// </summary>
        public string ApplicationVersion { get; set; }
    }
}
