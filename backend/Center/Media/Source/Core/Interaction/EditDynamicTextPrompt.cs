// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditDynamicTextPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The EditDynamicTextPrompt.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// the EditDynamicText Prompt
    /// </summary>
    public class EditDynamicTextPrompt : PromptNotification
    {
        private readonly ObservableCollection<TableDataViewModel> originalTables;

        private IMediaShell shell;

        private DynamicTextElementDataViewModel dynamicTextElement;

        private DictionaryDataViewModel dictionary;

        private ObservableCollection<TableDataViewModel> tables;

        private ExtendedObservableCollection<DictionaryValueDataViewModel> recentDictionaryValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditDynamicTextPrompt"/> class.
        /// </summary>
        /// <param name="dynamicTextElement">the referenced dynamic text element</param>
        /// <param name="shell">the shell</param>
        /// <param name="recentDictionaryValues">the recent dictionary value</param>
        public EditDynamicTextPrompt(
            DynamicTextElementDataViewModel dynamicTextElement,
            IMediaShell shell,
            ExtendedObservableCollection<DictionaryValueDataViewModel> recentDictionaryValues)
        {
            this.shell = shell;
            this.DynamicTextElement = dynamicTextElement;
            this.dictionary = this.shell.Dictionary;
            this.recentDictionaryValues = recentDictionaryValues;

            this.originalTables = new ExtendedObservableCollection<TableDataViewModel>();
            this.tables = new ExtendedObservableCollection<TableDataViewModel>();
            foreach (var table in this.dictionary.Tables)
            {
                this.originalTables.Add(table);
                this.tables.Add(table);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic text element
        /// </summary>
        public DynamicTextElementDataViewModel DynamicTextElement
        {
            get
            {
                return this.dynamicTextElement;
            }

            set
            {
                this.SetProperty(ref this.dynamicTextElement, value, () => this.DynamicTextElement);
            }
        }

        /// <summary>
        /// Gets or sets the Dictionary
        /// </summary>
        public DictionaryDataViewModel Dictionary
        {
            get
            {
                return this.dictionary;
            }

            set
            {
                this.SetProperty(ref this.dictionary, value, () => this.Dictionary);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic text element
        /// </summary>
        public IMediaShell Shell
        {
            get
            {
                return this.shell;
            }

            set
            {
                this.SetProperty(ref this.shell, value, () => this.Shell);
            }
        }

        /// <summary>
        /// Gets or sets the recent dictionary values
        /// </summary>
        public ExtendedObservableCollection<DictionaryValueDataViewModel> RecentDictionaryValues
        {
            get
            {
                return this.recentDictionaryValues;
            }

            set
            {
                this.SetProperty(ref this.recentDictionaryValues, value, () => this.RecentDictionaryValues);
            }
        }

        /// <summary>
        /// Gets or sets the recent dictionary tables
        /// </summary>
        public ObservableCollection<TableDataViewModel> DictionaryTables
        {
            get
            {
                return this.tables;
            }

            set
            {
                this.SetProperty(ref this.tables, value, () => this.DictionaryTables);
            }
        }

        /// <summary>
        /// filters the dictionary tables
        /// </summary>
        /// <param name="searchText">
        /// the search text
        /// </param>
        public void UpdateFilter(string searchText)
        {
            this.DictionaryTables.Clear();
            var searchTextLower = searchText.ToLower();

            foreach (var table in this.originalTables)
            {
                if (string.IsNullOrEmpty(searchTextLower)
                    || table.Columns.Any(c => c.Name.ToLower().Contains(searchTextLower)))
                {
                    table.Filter(searchText);
                    this.DictionaryTables.Add(table);
                }
            }
        }
    }
}