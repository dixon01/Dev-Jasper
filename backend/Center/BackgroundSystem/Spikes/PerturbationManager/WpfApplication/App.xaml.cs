// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the App type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfApplication
{
    using System;
    using System.Linq;
    using System.Windows;

    using WpfApplication.Model;
    using WpfApplication.ViewModels;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var commandLineParser = new CommandLineParser.CommandLineParser();
            var commandLine = new CommandLine();
            commandLineParser.ExtractArgumentAttributes(commandLine);
            commandLineParser.ParseCommandLine(e.Args);

            var shell = new Shell
                {
                    SelectedUnitId = 1
                };
            try
            {
                using (var dataContext = new DataContext())
                {
                    var displayedUnits =
                    dataContext.Units.OrderBy(unit => unit.Name)
                               .Select(
                                   unit =>
                                   new DisplayedUnit
                                       {
                                           Id = unit.Id,
                                           Name = unit.Name + " (" + unit.NetworkAddress + ")",
                                           IsSelected = unit.Name == "Labor VBSG"
                                       })
                               .ToList();
                    var selectedUnit = displayedUnits.SingleOrDefault(unit => unit.IsSelected);
                    if (selectedUnit != null)
                    {
                        shell.SelectedUnitId = selectedUnit.Id;
                    }

                    displayedUnits.ForEach(shell.Units.Add);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't load units. Exception: {0}", exception.Message);
                return;
            }

            if (commandLine.Start)
            {
                shell.Start();
            }

            var mainWindow = new MainWindow { DataContext = shell };
            mainWindow.ShowDialog();
        }
    }
}