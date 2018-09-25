// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DictionaryDataViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Dictionary
{
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// The DictionaryDataViewModel.
    /// </summary>
    public class DictionaryDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDataViewModel" /> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public DictionaryDataViewModel(Dictionary dictionary)
        {
            this.Languages = new List<LanguageDataViewModel>();
            this.Tables = new List<TableDataViewModel>();

            foreach (var language in dictionary.Languages)
            {
                this.Languages.Add(new LanguageDataViewModel(language));
            }

            foreach (var table in dictionary.Tables)
            {
                this.Tables.Add(new TableDataViewModel(table));
            }
        }

        /// <summary>
        /// Gets or sets the list of all <see cref="Language"/>s of the dictionary.
        /// </summary>
        public List<LanguageDataViewModel> Languages { get; set; }

        /// <summary>
        /// Gets or sets the list of all <see cref="Table"/>s of the dictionary.
        /// </summary>
        public List<TableDataViewModel> Tables { get; set; }
    }
}