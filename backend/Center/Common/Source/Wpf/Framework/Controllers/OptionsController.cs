// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionsController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the controller for application options
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Model;
    using Gorba.Center.Common.Wpf.Framework.Services;
    using Gorba.Center.Common.Wpf.Framework.Startup;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Defines the controller for application options
    /// </summary>
    public class OptionsController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string application;

        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsController"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        /// <param name="application">
        /// The application
        /// </param>
        /// <param name="name">
        /// The name of the file to save.
        /// </param>
        public OptionsController(ICommandRegistry commandRegistry, string application, string name)
        {
            commandRegistry.RegisterCommand(
                FrameworkCommandCompositionKeys.SaveOptions,
                new RelayCommand<OptionsPrompt>(this.Save));
            this.name = name;
            this.application = application;
        }

        private void Save(OptionsPrompt optionsPrompt)
        {
            var applicationState = ServiceLocator.Current.GetInstance<IApplicationState>();
            if (applicationState == null || applicationState.Options == null)
            {
                Logger.Warn("Can't save the application state.");
                return;
            }

            applicationState.Options = new ApplicationOptions();
            foreach (var category in optionsPrompt.Categories)
            {
                applicationState.Options.Categories.Add(category.CreateModel());
            }

            ApplicationStateManager.Current.Save(this.application, this.name, applicationState);
            Logger.Debug("Saved application options");
        }
    }
}
