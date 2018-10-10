// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryTriggerEditorSelectorPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DictionaryTriggerEditorSelectorPrompt.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The DictionaryTriggerEditorSelectorPrompt.
    /// </summary>
    public class DictionaryTriggerEditorSelectorPrompt : DictionarySelectorPrompt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryTriggerEditorSelectorPrompt"/> class.
        /// </summary>
        /// <param name="generic">
        /// The generic Value.
        /// </param>
        /// <param name="shell">
        /// the shell
        /// </param>
        /// <param name="recentDictionaryValues">
        /// the recent dictionary value
        /// </param>
        public DictionaryTriggerEditorSelectorPrompt(
            GenericEvalDataViewModel generic,
            IMediaShell shell,
            ExtendedObservableCollection<DictionaryValueDataViewModel> recentDictionaryValues)
            : base(generic.HumanReadable(), shell, recentDictionaryValues)
        {
        }
    }
}