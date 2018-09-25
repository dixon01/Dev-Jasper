// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectCycleTypePrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Prompt to select a cycle type and create this cycle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Collections.Generic;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.Resources;

    /// <summary>
    /// Prompt to select a cycle type and create this cycle.
    /// </summary>
    public class SelectCycleTypePrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCycleTypePrompt"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        public SelectCycleTypePrompt(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.CycleTypes = new List<string>
                                  {
                                      MediaStrings.CreateCyclePopup_StandardCycle,
                                      MediaStrings.CreateCyclePopup_EventCycle
                                  };
        }

        /// <summary>
        /// Gets the cycle types as localized strings.
        /// </summary>
        public List<string> CycleTypes { get; private set; }

        /// <summary>
        /// Gets the CreateCycle command.
        /// </summary>
        public ICommand CreateCycleCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Cycle.CreateNew);
            }
        }
    }
}
