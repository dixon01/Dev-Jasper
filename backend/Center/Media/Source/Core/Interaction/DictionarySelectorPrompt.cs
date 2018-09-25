// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionarySelectorPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The prompt to select a dictionary value.
    /// </summary>
    public class DictionarySelectorPrompt : PromptNotification
    {
        private readonly ObservableCollection<TableDataViewModel> originalTables;

        private readonly string dataValue;

        private IMediaShell shell;

        private DictionaryDataViewModel dictionary;

        private ObservableCollection<TableDataViewModel> tables;

        private ExtendedObservableCollection<DictionaryValueDataViewModel> recentDictionaryValues;

        private DictionaryValueDataViewModel selectedDictionaryValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionarySelectorPrompt"/> class.
        /// </summary>
        /// <param name="dataValue">
        /// The data Value.
        /// </param>
        /// <param name="shell">
        /// the shell
        /// </param>
        /// <param name="recentDictionaryValues">
        /// the recent dictionary value
        /// </param>
        public DictionarySelectorPrompt(
            string dataValue,
            IMediaShell shell,
            ExtendedObservableCollection<DictionaryValueDataViewModel> recentDictionaryValues)
        {
            this.shell = shell;
            this.dataValue = dataValue;
            this.dictionary = this.shell.Dictionary;
            this.recentDictionaryValues = recentDictionaryValues;
            this.selectedDictionaryValue = new DictionaryValueDataViewModel();
            this.originalTables = new ExtendedObservableCollection<TableDataViewModel>();
            this.tables = new ExtendedObservableCollection<TableDataViewModel>();
            foreach (var table in this.dictionary.Tables)
            {
                this.originalTables.Add(table);
                this.tables.Add(table);
            }

            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionarySelectorPrompt"/> class.
        /// </summary>
        /// <param name="dictionaryValue">
        /// The dictionary Value.
        /// </param>
        /// <param name="shell">
        /// the shell
        /// </param>
        /// <param name="recentDictionaryValues">
        /// the recent dictionary value
        /// </param>
        public DictionarySelectorPrompt(
            DictionaryValueDataViewModel dictionaryValue,
            IMediaShell shell,
            ExtendedObservableCollection<DictionaryValueDataViewModel> recentDictionaryValues)
        {
            this.shell = shell;
            this.dictionary = this.shell.Dictionary;
            this.recentDictionaryValues = recentDictionaryValues;
            this.selectedDictionaryValue = new DictionaryValueDataViewModel();
            this.originalTables = new ExtendedObservableCollection<TableDataViewModel>();
            this.tables = new ExtendedObservableCollection<TableDataViewModel>();
            foreach (var table in this.dictionary.Tables)
            {
                this.originalTables.Add(table);
                this.tables.Add(table);
            }

            if (dictionaryValue != null)
            {
                this.selectedDictionaryValue = dictionaryValue;
            }

            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the selected dictionary value.
        /// </summary>
        public DictionaryValueDataViewModel SelectedDictionaryValue
        {
            get
            {
                return this.selectedDictionaryValue;
            }

            set
            {
                this.SetProperty(ref this.selectedDictionaryValue, value, () => this.SelectedDictionaryValue);
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
        /// Gets or sets the placement target.
        /// </summary>
        /// <value>
        /// The placement target.
        /// </value>
        public IPlacementTarget PlacementTarget { get; set; }

        /// <summary>
        /// filters the dictionary tables
        /// </summary>
        /// <param name="searchText">
        /// the search text
        /// </param>
        public void UpdateFilter(string searchText)
        {
            this.DictionaryTables.Clear();

            foreach (var table in this.originalTables)
            {
                if (table.Columns.Any(c => c.Name.ToLower().Contains(searchText.ToLower())))
                {
                    table.Filter(searchText);
                    this.DictionaryTables.Add(table);
                }
            }
        }

        private void Initialize()
        {
            if (!string.IsNullOrEmpty(this.dataValue))
            {
                var applicationController = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
                GenericEvalDataViewModel genericEval;
                try
                {
                    var genericString = this.dataValue.Insert(0, "=");
                    genericEval =
                    applicationController.ShellController.FormulaController.ParseFormula(genericString) as
                    GenericEvalDataViewModel;
                }
                catch
                {
                    return;
                }

                if (genericEval == null)
                {
                    return;
                }

                this.selectedDictionaryValue.Row = genericEval.Row.Value;
                var language = this.dictionary.Languages.FirstOrDefault(l => l.Index == genericEval.Language.Value);
                this.selectedDictionaryValue.Language = language;
                var table = this.dictionary.Tables.FirstOrDefault(t => t.Index == genericEval.Table.Value);
                if (table == null)
                {
                    return;
                }

                this.selectedDictionaryValue.Table = table;
                var column = table.Columns.FirstOrDefault(c => c.Index == genericEval.Column.Value);
                if (column == null)
                {
                    return;
                }

                this.selectedDictionaryValue.Column = column;
            }
        }
    }
}
