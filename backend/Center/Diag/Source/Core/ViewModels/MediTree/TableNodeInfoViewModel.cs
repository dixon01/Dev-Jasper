// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableNodeInfoViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TableMediTreeNodeViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.MediTree
{
    using System.Data;

    using Gorba.Common.Medi.Core.Management.Remote;

    /// <summary>
    /// The view model representing table information in the Medi management tree.
    /// </summary>
    public class TableNodeInfoViewModel : NodeInfoViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableNodeInfoViewModel"/> class.
        /// </summary>
        /// <param name="provider">
        /// The table management provider represented by this view model.
        /// </param>
        public TableNodeInfoViewModel(IRemoteManagementTableProvider provider)
        {
            this.Table = new DataTable();

            var columnsSet = false;
            foreach (var row in provider.Rows)
            {
                if (!columnsSet)
                {
                    foreach (var property in row)
                    {
                        this.Table.Columns.Add(property.Name);
                    }

                    columnsSet = true;
                }

                var dataRow = this.Table.NewRow();
                foreach (var property in row)
                {
                    dataRow[property.Name] = property.StringValue;
                }

                this.Table.Rows.Add(dataRow);
            }
        }

        /// <summary>
        /// Gets the data table containing the data of the represented provider.
        /// </summary>
        public DataTable Table { get; private set; }
    }
}