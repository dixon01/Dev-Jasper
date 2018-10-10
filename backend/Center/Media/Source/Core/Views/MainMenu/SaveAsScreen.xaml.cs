// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveAsScreen.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for SaveAsScreen.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.MainMenu
{
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// Interaction logic for SaveAsScreen.xaml
    /// </summary>
    public partial class SaveAsScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveAsScreen"/> class.
        /// </summary>
        public SaveAsScreen()
        {
            this.InitializeComponent();
            this.Loaded += (sender, args) =>
                {
                    if (!(this.DataContext is SaveAsScreenViewModel context))
                    {
                        return;
                    }

                    context.SelectedTenant = context.Shell.MediaApplicationState.CurrentTenant;
                    if (context.Shell.MediaApplicationState.CurrentProject != null
                        && string.IsNullOrEmpty(context.Name))
                    {
                        context.Name = MediaStrings.ProjectController_CheckInAsNamePrefix
                                       + context.Shell.MediaApplicationState.CurrentProject.Name;
                    }
                };
        }

        /// <summary>
        /// Gets the save command wrapper.
        /// </summary>
        public ICommand SaveCommandWrapper => new RelayCommand(this.OnSave, this.CanSave);

        private bool CanSave(object o)
        {
            if (!(this.DataContext is SaveAsScreenViewModel context))
            {
                return false;
            }

            return context.SelectedTenant != null && !string.IsNullOrEmpty(context.Name)
                   && string.IsNullOrEmpty(context.Error);
        }

        private void OnSave()
        {
            if (!(this.DataContext is SaveAsScreenViewModel context))
            {
                return;
            }

            var parameters = new SaveAsParameters(context.Name, context.SelectedTenant);
            context.SaveAsCommand.Execute(parameters);
        }

        private void FilterTenantsSource(object sender, FilterEventArgs e)
        {
            if (!(e.Item is TenantReadableModel tenant))
            {
                e.Accepted = false;
                return;
            }

            if (!(this.DataContext is SaveAsScreenViewModel context))
            {
                e.Accepted = false;
                return;
            }

            if (context.Shell.PermissionController.HasPermission(
                tenant,
                Permission.Write,
                DataScope.MediaConfiguration))
            {
                e.Accepted = true;
            }
        }

        private void OnKeyUpHandleEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }
    }
}
