// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTtsElementDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The dynamic tts element data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The dynamic TTS element data view model.
    /// </summary>
    public class DynamicTtsElementDataViewModel : TextToSpeechElementDataViewModel
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> testData;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTtsElementDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        public DynamicTtsElementDataViewModel(IMediaShell mediaShell, DynamicTtsElementDataModel dataModel = null)
            : base(mediaShell, dataModel)
        {
            if (dataModel != null)
            {
                this.TestData = new DataValue<string>(dataModel.TestData);
            }
            else
            {
                this.TestData = new DataValue<string>(string.Empty);
            }

            this.mediaShell = mediaShell;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTtsElementDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        protected DynamicTtsElementDataViewModel(IMediaShell mediaShell, DynamicTtsElementDataViewModel dataViewModel)
            : base(mediaShell, dataViewModel)
        {
            this.TestData = new DataValue<string>(dataViewModel.TestData.Value);
        }

        /// <summary>
        /// Gets or sets the test data displayed in preview.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 3, GroupOrderIndex = 1)]
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

        /// <summary>
        /// Creates a deep clone of the current object.
        /// </summary>
        /// <returns>
        /// The deep cloned instance.
        /// </returns>
        public override object Clone()
        {
            var result = new DynamicTtsElementDataViewModel(this.mediaShell, this);
            if (this.IsDirty)
            {
                result.MakeDirty();
            }

            result.ClonedFrom = this.GetHashCode();

            return result;
        }

        /// <summary>
        /// Converts the DataViewModel to the DataModel.
        /// </summary>
        /// <returns>
        /// The converted <see cref="StaticTextElementDataModel"/>.
        /// </returns>
        public new DynamicTtsElementDataModel ToDataModel()
        {
            var model = (DynamicTtsElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(model);
            return model;
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

            var model = dataModel as DynamicTtsElementDataModel;
            if (model == null)
            {
                return;
            }

            model.TestData = this.TestData.Value;
        }

        /// <summary>
        /// Creates an empty <see cref="DynamicTextElementDataModel"/>.
        /// </summary>
        /// <returns>
        /// The the empty data model.
        /// </returns>
        protected override object CreateDataModelObject()
        {
            return new DynamicTtsElementDataModel();
        }

        private void TestDataChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.TestData);
        }
    }
}
