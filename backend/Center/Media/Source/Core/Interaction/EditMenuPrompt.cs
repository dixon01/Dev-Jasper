// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditMenuPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EditMenuPrompt type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// the edit menu prompt
    /// </summary>
    public class EditMenuPrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditMenuPrompt"/> class.
        /// </summary>
        /// <param name="commandRegistry">the command registry</param>
        public EditMenuPrompt(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets the Delete selected layout elements command
        /// </summary>
        public ICommand DeleteSelectedLayoutElements
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.DeleteSelectedElements);
            }
        }
    }
}