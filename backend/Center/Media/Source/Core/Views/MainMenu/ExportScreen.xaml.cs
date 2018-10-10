// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportScreen.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ExportScreen.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.MainMenu
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Interaction logic for ExportScreen.xaml
    /// </summary>
    public partial class ExportScreen
    {
        /// <summary>
        /// The exported event.
        /// </summary>
        public static readonly RoutedEvent ProjectExportedEvent = EventManager.RegisterRoutedEvent(
            "ProjectExported",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ExportScreen));

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportScreen"/> class.
        /// </summary>
        public ExportScreen()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The Project Exported Event Accessor
        /// </summary>
        public event RoutedEventHandler ProjectExported
        {
            add => this.AddHandler(ProjectExportedEvent, value);
 
            remove => this.RemoveHandler(ProjectExportedEvent, value);
        }

        /// <summary>
        /// Gets the export command wrapper.
        /// </summary>
        public ICommand ExportCommandWrapper
        {
            get
            {
                return new RelayCommand(this.OnExport, this.CanExport);
            }
        }

        private bool CanExport(object obj)
        {
            if (!(this.DataContext is ExportScreenViewModel context))
            {
                return false;
            }

            if (this.LocalExportTabItem.IsSelected)
            {
                return true;
            }

            if (this.ServerExportTabItem.IsSelected)
            {
                return context.UpdateGroups.Any(g => g.IsSelected)
                       && (!context.IsStartDateChecked || context.StartDate.HasValue)
                       && (!context.IsEndDateChecked || context.EndDate.HasValue)
                       && string.IsNullOrEmpty(context.Error);
            }

            if (this.TransferTabItem.IsSelected)
            {
                return true;
            }

            return false;
        }

        private void OnExport(object obj)
        {
            var context = (ExportScreenViewModel)this.DataContext;
            var controller = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
            controller.ShellController.ExportController.Exported += this.RaiseProjectExportedEvent;
            if (context == null)
            {
                return;
            }

            if (this.LocalExportTabItem.IsSelected)
            {
                context.ExportLocalCommand.Execute(null);
            }
            else if (this.ServerExportTabItem.IsSelected)
            {
                var start = context.StartDate ?? Settings.Default.UpdateStartDate;
                var end = context.EndDate ?? Settings.Default.UpdateEndDate;
                var exportParameters = new ExportParameters(
                    start,
                    end,
                    context.Description,
                    context.UpdateGroups.Where(u => u.IsSelected).Select(u => u));
                context.ExportServerCommand.Execute(exportParameters);
            }
            else if (this.TransferTabItem.IsSelected)
            {
                context.TransferProjectCommand.Execute(null);
            }
        }

        private void RaiseProjectExportedEvent(object sender, EventArgs e)
        {
            var controller = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
            controller.ShellController.ExportController.Exported -= this.RaiseProjectExportedEvent;
            var newEventArgs = new RoutedEventArgs(ProjectExportedEvent);
            this.RaiseEvent(newEventArgs);
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
