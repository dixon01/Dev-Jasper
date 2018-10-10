// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the App type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceBench
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Common.ServiceBench.ViewModels;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ServiceBenchController.RegisterServices();
            var assembly = this.GetType().Assembly;
            var assemblyCatalog = new AssemblyCatalog(assembly);
            var compositionContainer = new CompositionContainer(assemblyCatalog);
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var compositionBatch = new CompositionBatch();
            compositionBatch.AddExportedValue(taskScheduler);
            compositionContainer.Compose(compositionBatch);
            var shell = compositionContainer.GetExportedValue<Shell>();
            var window = new MainWindow { DataContext = shell };
            window.Show();
        }
    }
}