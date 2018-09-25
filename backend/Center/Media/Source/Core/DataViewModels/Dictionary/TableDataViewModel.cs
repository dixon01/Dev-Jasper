// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The TableDataViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Dictionary
{
    using System;
    using System.Runtime.Serialization;

    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models.Layout.Dictionary;
    using Gorba.Common.Protocols.Ximple.Generic;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The TableDataViewModel.
    /// </summary>
    [DataContract(Name = "Table", IsReference = true)]
    public class TableDataViewModel : ICloneable
    {
        private ExtendedObservableCollection<ColumnDataViewModel> originalColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDataViewModel"/> class.
        /// </summary>
        /// <param name="table">
        /// The <see cref="TableDataModel"/> used for initialization.
        /// </param>
        /// <remarks>
        /// This constructor is used when taking values from a serialized project.
        /// </remarks>
        public TableDataViewModel(TableDataModel table)
        {
            if (table == null)
            {
                this.Index = 0;
                this.Name = string.Empty;
                this.MultiLanguage = true;
                this.MultiRow = true;
                this.Columns = new ExtendedObservableCollection<ColumnDataViewModel>();
                return;
            }

            this.Index = table.Index;
            this.Name = table.Name;
            this.MultiLanguage = table.MultiLanguage;
            this.MultiRow = table.MultiRow;
            this.originalColumns = new ExtendedObservableCollection<ColumnDataViewModel>();
            this.Columns = new ExtendedObservableCollection<ColumnDataViewModel>();

            var dictionary = ServiceLocator.Current.GetInstance<DictionaryDataViewModel>();
            if (dictionary == null)
            {
                return;
            }

            var dictionaryTable = dictionary.Tables.Find(t => t.Index == table.Index);
            if (dictionaryTable != null)
            {
                foreach (var column in dictionaryTable.Columns)
                {
                    var columnDataViewModel = column;
                    this.originalColumns.Add(columnDataViewModel);
                    this.Columns.Add(columnDataViewModel);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDataViewModel"/> class.
        /// </summary>
        /// <param name="table">
        /// The <see cref="Table"/> used for initialization.
        /// </param>
        /// <remarks>
        /// This constructor is used when creating a new layout element.
        /// </remarks>
        public TableDataViewModel(Table table)
        {
            this.Index = table.Index;
            this.Name = table.Name;
            this.MultiLanguage = table.MultiLanguage;
            this.MultiRow = table.MultiRow;
            this.originalColumns = new ExtendedObservableCollection<ColumnDataViewModel>();
            this.Columns = new ExtendedObservableCollection<ColumnDataViewModel>();

            foreach (var column in table.Columns)
            {
                var columnDataViewModel = new ColumnDataViewModel(column, this);
                this.originalColumns.Add(columnDataViewModel);
                this.Columns.Add(columnDataViewModel);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDataViewModel"/> class.
        /// </summary>
        public TableDataViewModel()
        {
            this.Index = 0;
            this.Name = string.Empty;
            this.MultiLanguage = true;
            this.MultiRow = true;
            this.Columns = new ExtendedObservableCollection<ColumnDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the table index.
        /// </summary>
        [DataMember(Name = "Index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this table supports multiple languages.
        /// </summary>
        [DataMember(Name = "MultiLanguage")]
        public bool MultiLanguage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this table can contain multiple rows.
        /// </summary>
        [DataMember(Name = "MultiRow")]
        public bool MultiRow { get; set; }

        /// <summary>
        /// Gets or sets the description of the table.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the columns in the table.
        /// </summary>
        [DataMember(Name = "Columns")]
        public ExtendedObservableCollection<ColumnDataViewModel> Columns { get; set; }

        /// <summary>
        /// filters the Columns by a search text
        /// </summary>
        /// <param name="searchText">the search text</param>
        public void Filter(string searchText)
        {
            this.Columns.Clear();
            var searchTextLower = searchText.ToLower();

            foreach (var column in this.originalColumns)
            {
                if (string.IsNullOrEmpty(searchTextLower) || column.Name.ToLower().Contains(searchTextLower))
                {
                    this.Columns.Add(column);
                }
            }
        }

        /// <summary>
        /// Converts the view model to its data model representation.
        /// </summary>
        /// <returns>
        /// The data model representation of the view model.
        /// </returns>
        public TableDataModel ToDataModel()
        {
            var model = new TableDataModel();
            this.ConvertToDataModel(model);
            return model;
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Initializes the original columns according to the Columns property.
        /// </summary>
        public void InitializeOriginalColumns()
        {
            if (this.originalColumns == null)
            {
                this.originalColumns = new ExtendedObservableCollection<ColumnDataViewModel>();
            }

            this.originalColumns.Clear();
            this.originalColumns.AddRange(this.Columns);
        }

        /// <summary>
        /// Overrides the base conversion to DataModel adding specific conversion.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        protected void ConvertToDataModel(object dataModel)
        {
            var model = dataModel as TableDataModel;
            if (model == null)
            {
                return;
            }

            model.Index = this.Index;
            model.Name = this.Name;
            model.MultiLanguage = this.MultiLanguage;
            model.MultiRow = this.MultiRow;
        }
    }
}