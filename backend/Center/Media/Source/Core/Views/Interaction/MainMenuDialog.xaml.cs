// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainMenuDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainMenuDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// Interaction logic for MainMenuDialog.xaml
    /// </summary>
    public partial class MainMenuDialog
    {
        private Dictionary<string, ICommand> commandMapping;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuDialog"/> class.
        /// </summary>
        public MainMenuDialog()
        {
            this.InitializeComponent();
            this.MenuNavigation.SelectedItem = TabItemOpen;
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!(this.DataContext is MainMenuPrompt context))
            {
                return;
            }

            this.commandMapping = new Dictionary<string, ICommand>
                                  {
                                      {
                                          "Save",
                                          context.SaveProjectCommand
                                      },
                                      {
                                          "Exit", context.ExitCommand
                                      },
                                      {
                                          "CheckIn",
                                          context.CheckInCommand
                                      },
                                      {
                                          "About",
                                          context.ShowAboutScreenCommand
                                      },
                                      {
                                          "Options",
                                          context.ShowOptionsCommand
                                      }
                                  };

            context.NavigateTo = this.OnNavigateTo;
            if (context.Shell.MediaApplicationState.CurrentProject == null)
            {
                var selectedItem = this.MenuNavigation.SelectedItem as TabItem;
                if (selectedItem != null && selectedItem.Visibility == Visibility.Collapsed)
                {
                    var parameters = new MenuNavigationParameters
                                     {
                                         Root = MenuNavigationParameters.MainMenuEntries.FileOpen
                                     };
                    this.OnNavigateTo(parameters);
                }
            }
        }

        private void OnNavigateTo(object newSelection)
        {
            if (!(this.DataContext is MainMenuPrompt context))
            {
                return;
            }

            if (newSelection is TextualReplacementDataViewModel)
            {
                this.MenuNavigation.SelectedItem = this.TabItemTextualReplacement;
            }
            else if (newSelection is EvaluationConfigDataViewModel)
            {
                this.MenuNavigation.SelectedItem = this.TabItemFormulaManager;
                context.FormulaManagerPrompt.CurrentEvaluation = newSelection as EvaluationConfigDataViewModel;
            }
            else if (newSelection is MenuNavigationParameters)
            {
                var menuParameters = newSelection as MenuNavigationParameters;
                switch (menuParameters.Root)
                {
                    case MenuNavigationParameters.MainMenuEntries.FileNew:
                        this.MenuNavigation.SelectedItem = this.TabItemNew;
                        break;

                    case MenuNavigationParameters.MainMenuEntries.FileOpen:
                        this.MenuNavigation.SelectedItem = this.TabItemOpen;
                        break;

                    case MenuNavigationParameters.MainMenuEntries.FileImport:
                        this.MenuNavigation.SelectedItem = this.TabItemImport;
                        break;

                    case MenuNavigationParameters.MainMenuEntries.FileExport:
                        this.MenuNavigation.SelectedItem = this.TabItemExport;
                        break;

                    case MenuNavigationParameters.MainMenuEntries.FileSaveAs:
                        this.MenuNavigation.SelectedItem = this.TabItemSaveAs;
                        break;

                    case MenuNavigationParameters.MainMenuEntries.FileResourceManager:
                        this.MenuNavigation.SelectedItem = this.TabItemMedia;
                        if (menuParameters.SubMenu == "Fonts")
                        {
                            context.ResourceManagementPrompt.SelectedType = ResourceType.Font;
                        }

                        break;

                    case MenuNavigationParameters.MainMenuEntries.FileReplacement:
                        this.MenuNavigation.SelectedItem = this.TabItemTextualReplacement;
                        break;

                    case MenuNavigationParameters.MainMenuEntries.FileFormulaManager:
                        this.MenuNavigation.SelectedItem = this.TabItemFormulaManager;
                        break;
                }
            }
        }

        private void OnTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.commandMapping == null)
            {
                return;
            }

            if (e.AddedItems.Count == 1 && e.AddedItems[0] is TabItem)
            {
                var tabItem = (TabItem)e.AddedItems[0];
                var header = tabItem.Header.ToString();
                var headerGrid = tabItem.Header as Grid;
                if (headerGrid != null)
                {
                    header = headerGrid.Name;
                }

                if (this.commandMapping.ContainsKey(header))
                {
                    this.commandMapping[header].Execute(null);
                }

                if (this.IsSingleClickTab(tabItem))
                {
                    if (e.RemovedItems.Count > 0 && !this.IsSingleClickTab((TabItem)e.RemovedItems[0]))
                    {
                        this.MenuNavigation.SelectedItem = e.RemovedItems[0];
                    }
                    else
                    {
                        this.MenuNavigation.SelectedIndex = 0;
                    }

                    this.Close();
                }
            }
        }

        private bool IsSingleClickTab(TabItem tabItem)
        {
            var header = tabItem.Header.ToString();
            var headerGrid = tabItem.Header as Grid;
            if (headerGrid != null)
            {
                header = headerGrid.Name;
            }

            return header == "Save"
                   || header == "Exit"
                   || header == "CheckIn"
                   || header == "About"
                   || header == "Options";
        }

        private void OnProjectOpened(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
