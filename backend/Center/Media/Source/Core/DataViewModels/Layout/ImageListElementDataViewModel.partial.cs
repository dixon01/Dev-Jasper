// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageListElementDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The image list element data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;

    /// <summary>
    /// Defines the properties of an image list layout element.
    /// </summary>
    public partial class ImageListElementDataViewModel
    {
        private DataValue<string> testData;

        /// <summary>
        /// Gets or sets the test data.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 11, GroupOrderIndex = 1)]
        public DataValue<string> TestData
        {
            get
            {
                return this.testData;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.testData);
                if (this.testData != null)
                {
                    this.testData.PropertyChanged -= this.TestDataChanged;
                }

                this.SetProperty(ref this.testData, value, () => this.TestData);
                if (value != null)
                {
                    this.testData.PropertyChanged += this.TestDataChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        partial void Initialize(ImageListElementDataViewModel dataViewModel)
        {
            this.TestData = new DataValue<string>();
            if (dataViewModel != null)
            {
                this.TestData.Value = dataViewModel.TestData.Value;
            }
        }

        partial void Initialize(Models.Layout.ImageListElementDataModel dataModel)
        {
            this.TestData = new DataValue<string>();
            if (dataModel != null)
            {
                this.TestData.Value = dataModel.TestData;
            }
        }

        partial void ConvertNotGeneratedToDataModel(ref Models.Layout.ImageListElementDataModel dataModel)
        {
            dataModel.TestData = this.TestData.Value;
        }

        private void TestDataChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.TestData);
        }
    }
}
