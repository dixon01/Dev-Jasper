// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerVisualizerShell.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels
{
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Views;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore;

    /// <summary>
    /// Class to bind the views with data
    /// </summary>
    public class ComposerVisualizerShell : ViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        private MainView mainWindow;

        private PresentationTreeViewModel presentationTreeViewModel;

        private LayoutTabViewModel layoutViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposerVisualizerShell"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// the command registry
        /// </param>
        public ComposerVisualizerShell(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets or sets the presentation tree view model.
        /// </summary>
        public PresentationTreeViewModel PresentationTreeViewModel
        {
            get
            {
                return this.presentationTreeViewModel;
            }

            set
            {
                this.SetProperty(ref this.presentationTreeViewModel, value, () => this.PresentationTreeViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the layout view model.
        /// </summary>
        public LayoutTabViewModel LayoutViewModel
        {
            get
            {
                return this.layoutViewModel;
            }

            set
            {
                this.SetProperty(ref this.layoutViewModel, value, () => this.LayoutViewModel);
            }
        }

        /// <summary>
        /// Gets the exit command.
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                return this.commandRegistry.GetCommand("Exit");
            }
        }

        /// <summary>
        /// Gets the freeze command.
        /// </summary>
        public ICommand FreezeCommand
        {
            get
            {
                return this.commandRegistry.GetCommand("Freeze");
            }
        }

        /// <summary>
        /// Gets the presentation file name.
        /// </summary>
        public string PresentationFile { get; private set; }

        /// <summary>
        /// Creates the main window.
        /// </summary>
        /// <param name="layoutTab">
        ///     The layout Tab.
        /// </param>
        /// <param name="presentationFilename">the presentation file</param>
        public void CreateWindow(AllLayoutsView layoutTab, string presentationFilename)
        {
            this.PresentationFile = presentationFilename;
            this.mainWindow = new MainView();
            var tab = new TabItem();
            tab.Header = "Layouts";
            tab.Content = layoutTab;
            this.mainWindow.MainTabControl.Items.Insert(1, tab);
            this.mainWindow.Show();
            this.mainWindow.DataContext = this;
        }
    }
}
