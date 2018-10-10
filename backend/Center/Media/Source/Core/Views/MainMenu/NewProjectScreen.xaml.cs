// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewProjectScreen.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for NewProjectScreen.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.MainMenu
{
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// Interaction logic for NewProjectScreen.xaml
    /// </summary>
    public partial class NewProjectScreen
    {
        /// <summary>
        /// The created event.
        /// </summary>
        public static readonly RoutedEvent ProjectCreatedEvent = EventManager.RegisterRoutedEvent(
            "ProjectCreated",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(NewProjectScreen));

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectScreen"/> class.
        /// </summary>
        public NewProjectScreen()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The Project Exported Event Accessor
        /// </summary>
        public event RoutedEventHandler ProjectCreated
        {
            add => this.AddHandler(ProjectCreatedEvent, value);

            remove => this.RemoveHandler(ProjectCreatedEvent, value);
        }

        /// <summary>
        /// Gets the create new project command wrapper to be able to close the popup after the command execution.
        /// </summary>
        public ICommand CreateNewProjectCommandWrapper =>
            new RelayCommand<CreatePhysicalScreenParameters>(this.CreateNewProject, this.CanCreateNewProject);

        private void CreateNewProject(CreatePhysicalScreenParameters screenParameters)
        {
            var context = (MainMenuPrompt)this.DataContext;
            var projectParameters = new CreateProjectParameters
            {
                Description = context.Description,
                Name = context.NewProjectName,
                IsMonochrome = screenParameters.IsMonochrome,
                MasterLayout = screenParameters.MasterLayout,
                Resolution = screenParameters.Resolution,
                Type = screenParameters.Type,
            };

            context.CreateNewProjectCommand.Execute(projectParameters);

            var newEventArgs = new RoutedEventArgs(ProjectCreatedEvent);
            this.RaiseEvent(newEventArgs);
        }

        private bool CanCreateNewProject(object obj)
        {
            return this.DataContext is MainMenuPrompt context && string.IsNullOrEmpty(context.Error)
                   && !string.IsNullOrEmpty(context.NewProjectName);
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
