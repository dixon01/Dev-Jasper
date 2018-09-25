// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ApplicationController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Gorba.Center.Common.Wpf.Framework.Model;
    using Gorba.Center.Common.Wpf.Framework.Model.Options;
    using Gorba.Center.Common.Wpf.Framework.Startup;

    using NLog;

    /// <summary>
    /// Defines an abstract base <see cref="IApplicationController"/>.
    /// </summary>
    public abstract class ApplicationController : IApplicationController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Occurs when the application shutdown is requested by the controller.
        /// </summary>
        public event EventHandler ShutdownCompleted;

        /// <summary>
        /// Initializes this controller.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Runs the controller logic until completed or until the <see cref="Shutdown"/>.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Requests the shutdown of this controller.
        /// </summary>
        public abstract void Shutdown();

        /// <summary>
        /// Raises the shutdown completed event.
        /// </summary>
        protected virtual void RaiseShutdownCompleted()
        {
            var handler = this.ShutdownCompleted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Initializes the general options of all applications
        /// </summary>
        /// <param name="state">
        /// The application state.
        /// </param>
        protected virtual void InitializeApplicationStateOptions(IApplicationState state)
        {
            if (state == null)
            {
                return;
            }

            if (state.Options == null || state.Options.Categories.Count <= 0)
            {
                Logger.Warn("No options found in ApplicationState. Creating default ones.");
                state.Options = this.CreateApplicationOptions();
            }
            else
            {
                try
                {
                    var languageGroup = state.Options.Categories.First().Groups.First() as LanguageOptionGroup;
                    if (languageGroup == null)
                    {
                        return;
                    }

                    var currentLanguage = new CultureInfo(languageGroup.Language);
                    Thread.CurrentThread.CurrentCulture = currentLanguage;
                    Thread.CurrentThread.CurrentUICulture = currentLanguage;
                }
                catch (Exception e)
                {
                    Logger.Warn("Couldn't configure application options. Creating default ones.", e);
                    this.CreateApplicationOptions();
                }
            }
        }

        private ApplicationOptions CreateApplicationOptions()
        {
            var options = new ApplicationOptions();
            var generalCategory = new GeneralOptionCategory();
            var languageGroup = new LanguageOptionGroup { Language = "en-US" };
            generalCategory.Groups.Add(languageGroup);
            options.Categories.Add(generalCategory);
            return options;
        }
    }
}