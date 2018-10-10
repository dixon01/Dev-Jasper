// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ColumnDataViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Dictionary
{
    using System;
    using System.Runtime.Serialization;

    using Gorba.Center.Media.Core.Models.Layout.Dictionary;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// The ColumnDataViewModel.
    /// </summary>
    [DataContract(Name = "Column", IsReference = true)]
    public class ColumnDataViewModel : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDataViewModel" /> class.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="table">
        /// The table.
        /// </param>
        public ColumnDataViewModel(Column column, TableDataViewModel table)
        {
            this.Index = column.Index;
            this.Name = column.Name;
            this.Table = table;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDataViewModel"/> class.
        /// </summary>
        /// <param name="columnDataModel">
        /// The column data model.
        /// </param>
        /// <param name="table">
        /// The table.
        /// </param>
        public ColumnDataViewModel(ColumnDataModel columnDataModel, TableDataViewModel table)
        {
            this.Index = columnDataModel.Index;
            this.Name = columnDataModel.Name;
            this.Table = table;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDataViewModel" /> class.
        /// </summary>
        public ColumnDataViewModel()
        {
            this.Index = 0;
            this.Name = string.Empty;
        }

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        [DataMember(Name = "Index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the column.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the table reference
        /// </summary>
        [DataMember(Name = "Table")]
        public TableDataViewModel Table { get; set; }

        /// <summary>
        /// Converts the view model to its data model representation.
        /// </summary>
        /// <returns>
        /// The data model representation of the view model.
        /// </returns>
        public ColumnDataModel ToDataModel()
        {
            var model = new ColumnDataModel();
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
            var viewModel = new ColumnDataViewModel
                                {
                                    Description = this.Description,
                                    Index = this.Index,
                                    Name = this.Name,
                                    Table = (TableDataViewModel)this.Table.Clone()
                                };
            return viewModel;
        }

        /// <summary>
        /// Overrides the base conversion to DataModel adding specific conversion.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        protected void ConvertToDataModel(object dataModel)
        {
            var model = dataModel as ColumnDataModel;
            if (model == null)
            {
                return;
            }

            model.Index = this.Index;
            model.Name = this.Name;
            model.Table = this.Table.ToDataModel();
        }
    }
}