// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryValueDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DictionaryValueDataViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.Linq;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The DictionaryValueDataViewModel.
    /// </summary>
    public class DictionaryValueDataViewModel : DataViewModelBase, ICloneable
    {
        private TableDataViewModel table;

        private ColumnDataViewModel column;

        private int row;

        private LanguageDataViewModel language;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryValueDataViewModel"/> class.
        /// </summary>
        public DictionaryValueDataViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryValueDataViewModel"/> class.
        /// </summary>
        /// <param name="dataModel">
        /// The data model used for initialization.
        /// </param>
        public DictionaryValueDataViewModel(DictionaryValueElementDataModel dataModel)
        {
            if (dataModel == null)
            {
                return;
            }

            var dictionary = ServiceLocator.Current.GetInstance<DictionaryDataViewModel>();
            if (dataModel.Table == null)
            {
                this.Table = new TableDataViewModel();
            }
            else
            {
                var dictTable =
                    dictionary.Tables.Find(t => t.Name == dataModel.Table.Name && t.Index == dataModel.Table.Index);
                this.Table = dictTable ?? new TableDataViewModel(dataModel.Table);
            }

            if (dataModel.Column == null)
            {
                this.Column = new ColumnDataViewModel();
            }
            else
            {
                var dictColumn =
                    this.Table.Columns.SingleOrDefault(
                        c => c.Name == dataModel.Column.Name && c.Index == dataModel.Column.Index);
                this.Column = dictColumn ?? new ColumnDataViewModel(dataModel.Column, this.table);
            }

            this.Row = dataModel.Row;
            if (dataModel.Language == null)
            {
                this.Language = new LanguageDataViewModel();
            }
            else
            {
                var dictLanguage = dictionary.Languages.Find(l => l.Name == dataModel.Language.Name);
                this.Language = dictLanguage ?? new LanguageDataViewModel(dataModel.Language);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic text.
        /// </summary>
        [UserVisibleProperty("Content")]
        public TableDataViewModel Table
        {
            get
            {
                return this.table;
            }

            set
            {
                this.SetProperty(ref this.table, value, () => this.Table);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic text.
        /// </summary>
        [UserVisibleProperty("Content")]
        public ColumnDataViewModel Column
        {
            get
            {
                return this.column;
            }

            set
            {
                this.SetProperty(ref this.column, value, () => this.Column);
            }
        }

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        [UserVisibleProperty("Content")]
        public LanguageDataViewModel Language
        {
            get
            {
                return this.language;
            }

            set
            {
                this.SetProperty(ref this.language, value, () => this.Language);
            }
        }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        [UserVisibleProperty("Content")]
        public int Row
        {
            get
            {
                return this.row;
            }

            set
            {
                this.SetProperty(ref this.row, value, () => this.Row);
            }
        }

        /// <summary>
        /// Converts the view model to its data model representation.
        /// </summary>
        /// <returns>
        /// The data model representation of the view model.
        /// </returns>
        public DictionaryValueElementDataModel ToDataModel()
        {
            var model = new DictionaryValueElementDataModel();
            this.ConvertToDataModel(model);
            return model;
        }

        /// <summary>
        /// Creates a deep clone of the current object.
        /// </summary>
        /// <returns>
        /// The deep cloned instance.
        /// </returns>
        public object Clone()
        {
            var result = new DictionaryValueDataViewModel
            {
                Table = this.Table,
                Column = this.Column,
                Row = this.Row,
                Language = this.Language,
            };
            return result;
        }

        /// <summary>
        /// Overrides the base conversion to DataModel adding specific conversion.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        protected override void ConvertToDataModel(object dataModel)
        {
            base.ConvertToDataModel(dataModel);

            var model = dataModel as DictionaryValueElementDataModel;
            if (model == null)
            {
                return;
            }

            model.Row = this.Row;
            if (this.Language != null)
            {
                model.Language = this.Language.ToDataModel();
            }

            if (this.Table != null)
            {
                model.Table = this.Table.ToDataModel();
            }

            if (this.Column == null)
            {
                return;
            }

            model.Column = this.Column.ToDataModel();
        }
    }
}