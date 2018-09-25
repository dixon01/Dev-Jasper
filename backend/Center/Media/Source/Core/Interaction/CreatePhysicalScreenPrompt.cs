// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreatePhysicalScreenPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The create physical screen prompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.Configuration;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The create physical screen prompt.
    /// </summary>
    public class CreatePhysicalScreenPrompt : PromptNotification
    {
        private readonly Lazy<MediaConfiguration> lazyMediaConfiguration;

        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePhysicalScreenPrompt"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        public CreatePhysicalScreenPrompt(ICommandRegistry commandRegistry)
        {
            this.lazyMediaConfiguration = new Lazy<MediaConfiguration>(GetMediaConfiguration);
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets the create physical screen command.
        /// </summary>
        public ICommand CreatePhysicalScreenCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.PhysicalScreen.CreateNew);
            }
        }

        /// <summary>
        /// Gets the media configuration.
        /// </summary>
        public MediaConfiguration MediaConfiguration
        {
            get
            {
                return this.lazyMediaConfiguration.Value;
            }
        }

        private static MediaConfiguration GetMediaConfiguration()
        {
            return ServiceLocator.Current.GetInstance<MediaConfiguration>();
        }
    }
}
