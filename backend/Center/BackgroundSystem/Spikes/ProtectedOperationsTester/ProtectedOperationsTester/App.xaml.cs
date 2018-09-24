using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ProtectedOperationsTester
{
    using ProtectedOperationsTester.Core;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper();
            var shell = bootstrapper.Bootstrap();
            var mainWindow = new MainWindow { DataContext = shell };
            mainWindow.ShowDialog();
        }
    }
}
