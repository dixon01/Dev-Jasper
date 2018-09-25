// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTextElementDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Defines the properties of a dynamic text layout element.
    /// </summary>
    public class DynamicTextElementDataViewModel : TextElementDataViewModel
    {
        private DictionaryValueDataViewModel selectedDictionaryValue;

        private DataValue<string> testData;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTextElementDataViewModel"/> class.
        /// </summary>
        /// <param name="parentViewModel">
        /// The parent view model.
        /// </param>
        public DynamicTextElementDataViewModel(IMediaShell parentViewModel)
            : base(parentViewModel)
        {
            this.SelectedDictionaryValue = new DictionaryValueDataViewModel
                                           {
                                               Table =
                                                   parentViewModel.Dictionary.Tables
                                                   .FirstOrDefault()
                                           };

            if (this.SelectedDictionaryValue.Table != null)
            {
                this.SelectedDictionaryValue.Column = this.SelectedDictionaryValue.Table.Columns.FirstOrDefault();
            }
            else
            {
                throw new Exception("Could not get table from dictionary");
            }

            this.TestData = new DataValue<string>(string.Empty);
            this.PropertyChanged += this.OnBasePropertyChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTextElementDataViewModel"/> class.
        /// </summary>
        /// <param name="parentViewModel">
        /// The parent view model.
        /// </param>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        public DynamicTextElementDataViewModel(IMediaShell parentViewModel, DynamicTextElementDataModel dataModel)
            : base(parentViewModel, dataModel)
        {
            this.TestData = new DataValue<string>(string.Empty);

            if (dataModel != null)
            {
                this.SelectedDictionaryValue = new DictionaryValueDataViewModel(dataModel.SelectedDictionaryValue);
                this.TestData.Value = dataModel.TestData;
            }
            else
            {
                this.SelectedDictionaryValue = new DictionaryValueDataViewModel();
            }

            this.PropertyChanged += this.OnBasePropertyChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTextElementDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        protected DynamicTextElementDataViewModel(IMediaShell mediaShell, DynamicTextElementDataViewModel dataViewModel)
            : base(mediaShell, dataViewModel)
        {
            this.SelectedDictionaryValue = (DictionaryValueDataViewModel)dataViewModel.SelectedDictionaryValue.Clone();
            this.TestData = new DataValue<string>(dataViewModel.TestData.Value);
        }

        /// <summary>
        /// Gets or sets the dynamic text.
        /// </summary>
        public DictionaryValueDataViewModel SelectedDictionaryValue
        {
            get
            {
                return this.selectedDictionaryValue;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.selectedDictionaryValue);
                if (this.selectedDictionaryValue != null)
                {
                    this.selectedDictionaryValue.PropertyChanged -= this.SelectedDictionaryValueChanged;
                }

                this.SetProperty(ref this.selectedDictionaryValue, value, () => this.SelectedDictionaryValue);
                if (value != null)
                {
                    this.selectedDictionaryValue.PropertyChanged += this.SelectedDictionaryValueChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the test data displayed in preview.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 6, GroupOrderIndex = 1)]
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
            var result = new DynamicTextElementDataViewModel(this.Shell, this);
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
        public new DynamicTextElementDataModel ToDataModel()
        {
            var model = (DynamicTextElementDataModel)this.CreateDataModelObject();
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

            var model = dataModel as DynamicTextElementDataModel;
            if (model == null)
            {
                return;
            }

            model.SelectedDictionaryValue = this.SelectedDictionaryValue.ToDataModel();
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
            return new DynamicTextElementDataModel();
        }

        private void SelectedDictionaryValueChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.SelectedDictionaryValue);
        }

        private void TestDataChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.TestData);
        }

        private void ValueFormulaChanged(object sender, PropertyChangedEventArgs e)
        {
            var generic = this.Value.Formula as GenericEvalDataViewModel;
            if (generic == null)
            {
                return;
            }

            if (e.PropertyName == "Table")
            {
                this.selectedDictionaryValue.Table =
                     this.Parent.Parent.Dictionary.Tables.FirstOrDefault(t => t.Index == generic.Table.Value);
                if (this.selectedDictionaryValue.Table != null)
                {
                    this.selectedDictionaryValue.Column =
                        this.selectedDictionaryValue.Table.Columns.FirstOrDefault(c => c.Index == generic.Column.Value);
                }

                return;
            }

            if (e.PropertyName == "Column")
            {
                if (this.selectedDictionaryValue.Table != null)
                {
                    this.selectedDictionaryValue.Column =
                        this.selectedDictionaryValue.Table.Columns.FirstOrDefault(c => c.Index == generic.Column.Value);
                }

                return;
            }

            if (e.PropertyName == "Language")
            {
                this.selectedDictionaryValue.Language =
                     this.Parent.Parent.Dictionary.Languages.FirstOrDefault(l => l.Index == generic.Language.Value);
                return;
            }

            if (e.PropertyName == "Row")
            {
                this.selectedDictionaryValue.Row = generic.Row.Value;
            }
        }

        private void OnBasePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                if (Value.Formula != null)
                {
                    this.Value.Formula.PropertyChanged -= this.ValueFormulaChanged;
                    var generic = this.Value.Formula as GenericEvalDataViewModel;
                    if (generic == null)
                    {
                        return;
                    }

                    this.selectedDictionaryValue.Table =
                        this.Parent.Parent.Dictionary.Tables.FirstOrDefault(t => t.Index == generic.Table.Value);
                    if (this.selectedDictionaryValue.Table != null)
                    {
                        this.selectedDictionaryValue.Column =
                            this.selectedDictionaryValue.Table.Columns.FirstOrDefault(
                                c => c.Index == generic.Column.Value);
                    }

                    this.selectedDictionaryValue.Language =
                        this.Parent.Parent.Dictionary.Languages.FirstOrDefault(l => l.Index == generic.Language.Value);
                    this.selectedDictionaryValue.Row = generic.Row.Value;
                    this.Value.Formula.PropertyChanged += this.ValueFormulaChanged;
                }
            }
        }
    }
}
