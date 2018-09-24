// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantSelectionController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The tenant selection controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using System;
    using System.Windows;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The tenant selection controller.
    /// </summary>
    public class TenantSelectionController : DialogControllerBase, ITenantSelectionController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantSelectionController"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        /// <param name="window">
        /// The window.
        /// </param>
        public TenantSelectionController(
            ICommandRegistry commandRegistry,
            TenantSelectionViewModel window)
            : base(window)
        {
            commandRegistry.RegisterCommand(
              ClientCommandCompositionKeys.TenantSelection,
              new RelayCommand<TenantReadableModel>(this.SelectTenant, this.CanSelectTenant));
            commandRegistry.RegisterCommand(
                ClientCommandCompositionKeys.CancelTenantSelection,
                new RelayCommand(this.CancelTenantSelection));
        }

        /// <summary>
        /// Gets the tenant selection.
        /// </summary>
        public TenantSelectionViewModel TenantSelection
        {
            get
            {
                return this.Dialog as TenantSelectionViewModel;
            }
        }

        /// <summary>
        /// Gets the tenant selection dialog result.
        /// </summary>
        public TenantSelectionDialogResult TenantSelectionDialogResult { get; private set; }

        /// <summary>
        /// The run.
        /// </summary>
        /// <returns>
        /// The <see cref="DialogResultBase"/>.
        /// </returns>
        public override DialogResultBase Run()
        {
            var state = ServiceLocator.Current.GetInstance<IApplicationState>();
            this.TenantSelection.SetMainScreen(state);
            this.TenantSelection.Closing += this.TenantSelectionOnClosing;
            var dialog = this.TenantSelection.ShowDialog();
            this.TenantSelection.Closing -= this.TenantSelectionOnClosing;
            if (dialog.HasValue && dialog.Value && this.TenantSelectionDialogResult != null)
            {
                return this.TenantSelectionDialogResult;
            }

            return new EmptyDialogResult();
        }

        private void TenantSelectionOnClosing(object sender, EventArgs eventArgs)
        {
            this.TenantSelection.ApplicationState.UpdateMainScreen((Window)this.TenantSelection.Dialog);
        }

        private void CancelTenantSelection()
        {
            this.Dialog.DialogResult = false;
        }

        private void SelectTenant(TenantReadableModel tenant)
        {
            if (tenant != null)
            {
                this.TenantSelectionDialogResult = new TenantSelectionDialogResult(tenant);
                this.TenantSelection.ApplicationState.CurrentTenant = tenant;
                this.Dialog.DialogResult = true;
            }
        }

        private bool CanSelectTenant(object parameter)
        {
            return this.TenantSelection.SelectedTenant != null;
        }
    }
}