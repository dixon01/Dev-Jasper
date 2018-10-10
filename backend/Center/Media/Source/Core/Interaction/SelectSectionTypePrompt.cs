// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectSectionTypePrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The select section type prompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Collections.Generic;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;

    /// <summary>
    /// The ViewModel for the SelectSectionType view.
    /// </summary>
    public class SelectSectionTypePrompt : PromptNotification
    {
          private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectSectionTypePrompt"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        /// <param name="layouts">
        /// The layouts.
        /// </param>
        public SelectSectionTypePrompt(ICommandRegistry commandRegistry, IEnumerable<LayoutConfigDataViewModel> layouts)
        {
            this.commandRegistry = commandRegistry;
            this.Layouts = layouts;
        }

        /// <summary>
        /// Gets the list of Layouts
        /// </summary>
        public IEnumerable<LayoutConfigDataViewModel> Layouts { get; private set; }

        /// <summary>
        /// Gets the CreateCycle command.
        /// </summary>
        public ICommand CreateSectionCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Section.CreateNew);
            }
        }
    }
}
