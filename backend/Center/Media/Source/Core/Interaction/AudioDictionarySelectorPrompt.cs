// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioDictionarySelectorPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The audio dictionary selector prompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The audio dictionary selector prompt.
    /// </summary>
    public class AudioDictionarySelectorPrompt : DictionarySelectorPrompt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDictionarySelectorPrompt"/> class.
        /// </summary>
        /// <param name="dataValue">
        /// The data value.
        /// </param>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="recentDictionaryValues">
        /// The recent dictionary values.
        /// </param>
        public AudioDictionarySelectorPrompt(
            string dataValue,
            IMediaShell shell,
            ExtendedObservableCollection<DictionaryValueDataViewModel> recentDictionaryValues)
            : base(dataValue, shell, recentDictionaryValues)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDictionarySelectorPrompt"/> class.
        /// </summary>
        /// <param name="dictionaryValue">
        /// The dictionary value.
        /// </param>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="recentDictionaryValues">
        /// The recent dictionary values.
        /// </param>
        public AudioDictionarySelectorPrompt(
            DictionaryValueDataViewModel dictionaryValue,
            IMediaShell shell,
            ExtendedObservableCollection<DictionaryValueDataViewModel> recentDictionaryValues)
            : base(dictionaryValue, shell, recentDictionaryValues)
        {
        }
    }
}
