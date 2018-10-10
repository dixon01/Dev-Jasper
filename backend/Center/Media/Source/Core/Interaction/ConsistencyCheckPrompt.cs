// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsistencyCheckPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// The consistency check prompt.
    /// </summary>
    public class ConsistencyCheckPrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsistencyCheckPrompt"/> class.
        /// </summary>
        /// <param name="consistencyMessages">
        /// The consistency messages.
        /// </param>
        /// <param name="compatibilityMessages">
        /// The compatibility messages.
        /// </param>
        /// <param name="commandRegistry">
        /// command registry
        /// </param>
        public ConsistencyCheckPrompt(
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> consistencyMessages,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> compatibilityMessages,
            ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.ConsistencyMessages = consistencyMessages;
            this.CompatibilityMessages = compatibilityMessages
                                         ?? new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();
        }

        /// <summary>
        /// Gets the consistency messages.
        /// </summary>
        public ExtendedObservableCollection<ConsistencyMessageDataViewModel> ConsistencyMessages { get; private set; }

        /// <summary>
        /// Gets the compatibility messages.
        /// </summary>
        public ExtendedObservableCollection<ConsistencyMessageDataViewModel> CompatibilityMessages { get; private set; }

        /// <summary>
        /// Gets the Navigate Command
        /// </summary>
        public ICommand NavigateToConsistencyProblem
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Navigation.GoToConsistencyProblem);
            }
        }
    }
}
