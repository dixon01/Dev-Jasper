using System.Linq;
using System.Windows;
using Library.Client;

namespace WpfApplication
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Controls;

    using Library.Tracking;
    using Library.ViewModel;

    using WpfApplication.Controllers;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Shell shell;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await TenantsController.Instance.Load();
        }

        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
            TenantsController.Instance.Update();
        }

        private void Edit_OnClick(object sender, RoutedEventArgs e)
        {
            TenantsController.Instance.Edit();
            //await this.shell.SelectedTenant.Update();
        }

        private ReadOnlyTenantDataViewModel Convert(TenantReadableModel model)
        {
            return new ReadOnlyTenantDataViewModel(model);
        }
    }
}
