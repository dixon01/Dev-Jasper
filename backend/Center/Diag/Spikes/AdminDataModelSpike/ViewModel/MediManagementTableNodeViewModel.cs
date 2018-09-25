namespace AdminDataModelSpike.ViewModel
{
    using System;
    using System.Data;

    using Gorba.Common.Medi.Core.Management;

    public class MediManagementTableNodeViewModel : MediManagementDataNodeViewModelBase
    {
        private readonly IManagementTableProvider provider;

        private DataTable table;

        public MediManagementTableNodeViewModel(IManagementTableProvider provider, MediManagementTreeViewModel owner)
            : base(provider, owner)
        {
            this.provider = provider;
            this.Table = new DataTable();
        }

        public DataTable Table
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

        protected override Action LoadData()
        {
            var newTable = new DataTable();
            foreach (var row in this.provider.Rows)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    if (newTable.Columns.Count <= i)
                    {
                        newTable.Columns.Add(row[i].Name);
                    }
                }

                var dataRow = newTable.NewRow();
                foreach (var cell in row)
                {
                    dataRow[cell.Name] = cell.StringValue;
                }

                newTable.Rows.Add(dataRow);
            }

            return () => this.Table = newTable;
        }
    }
}