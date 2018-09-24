// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RssTickerElementDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RssTickerElementDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// The RSS ticker element data view model.
    /// </summary>
    public class RssTickerElementDataViewModel : DrawableElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<HorizontalAlignment> align;

        private DataValue<VerticalAlignment> valign;

        private DataValue<int> scrollspeed;

        private FontDataViewModel font;

        private DataValue<string> testData;

        private DataValue<string> rssUrl;
        private DataValue<string> delimiter;
        private DataValue<TimeSpan> updateInterval;
        private DataValue<TimeSpan> validity;

        /// <summary>
        /// Initializes a new instance of the <see cref="RssTickerElementDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        public RssTickerElementDataViewModel(IMediaShell mediaShell, RssTickerElementDataModel dataModel = null)
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Align = new DataValue<HorizontalAlignment>(new HorizontalAlignment());
            this.Align.PropertyChanged += this.OnAlignChanged;
            this.VAlign = new DataValue<VerticalAlignment>(VerticalAlignment.Top);
            this.VAlign.PropertyChanged += this.OnVAlignChanged;
            this.ScrollSpeed = new DataValue<int>(-200);
            this.ScrollSpeed.PropertyChanged += this.OnScrollSpeedChanged;
            this.font = new FontDataViewModel(this.mediaShell);
            this.TestData = new DataValue<string>();
            this.TestData.PropertyChanged += this.OnTestDataChanged;
            this.RssUrl = new DataValue<string>();
            this.RssUrl.PropertyChanged += this.OnRssUrlChanged;
            this.Delimiter = new DataValue<string>();
            this.Delimiter.PropertyChanged += this.OnDelimiterChanged;
            this.UpdateInterval = new DataValue<TimeSpan>(TimeSpan.FromMinutes(5));
            this.UpdateInterval.PropertyChanged += this.OnUpdateIntervalChanged;
            this.Validity = new DataValue<TimeSpan>(TimeSpan.FromMinutes(1439));
            this.Validity.PropertyChanged += this.OnValidityChanged;
            if (dataModel != null)
            {
                this.Align.Value = dataModel.Align;
                this.VAlign.Value = dataModel.VAlign;
                this.ScrollSpeed.Value = dataModel.ScrollSpeed;
                this.font = new FontDataViewModel(this.mediaShell, dataModel.Font);
                this.TestData.Value = dataModel.TestData;
                this.Delimiter.Value = dataModel.Delimiter;
                this.RssUrl.Value = dataModel.RssUrl;
                this.UpdateInterval.Value = dataModel.UpdateInterval;
                this.Validity.Value = dataModel.Validity;
                this.ExportingRow = dataModel.ExportingRow;
                this.DisplayText = dataModel.DisplayText;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RssTickerElementDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        protected RssTickerElementDataViewModel(IMediaShell mediaShell, RssTickerElementDataViewModel dataViewModel)
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Align = (DataValue<HorizontalAlignment>)dataViewModel.Align.Clone();
            this.Align.PropertyChanged += this.OnAlignChanged;
            this.VAlign = (DataValue<VerticalAlignment>)dataViewModel.VAlign.Clone();
            this.VAlign.PropertyChanged += this.OnVAlignChanged;
            this.ScrollSpeed = (DataValue<int>)dataViewModel.ScrollSpeed.Clone();
            this.ScrollSpeed.PropertyChanged += this.OnScrollSpeedChanged;
            var clonedFont = dataViewModel.Font;
            if (clonedFont != null)
            {
                this.Font = (FontDataViewModel)clonedFont.Clone();
            }

            this.TestData = (DataValue<string>)dataViewModel.TestData.Clone();
            this.TestData.PropertyChanged += this.OnTestDataChanged;
            this.Delimiter = (DataValue<string>)dataViewModel.Delimiter.Clone();
            this.Delimiter.PropertyChanged += this.OnDelimiterChanged;
            this.RssUrl = (DataValue<string>)dataViewModel.RssUrl.Clone();
            this.RssUrl.PropertyChanged += this.OnRssUrlChanged;
            this.UpdateInterval = (DataValue<TimeSpan>)dataViewModel.UpdateInterval.Clone();
            this.UpdateInterval.PropertyChanged += this.OnUpdateIntervalChanged;
            this.Validity = (DataValue<TimeSpan>)dataViewModel.Validity.Clone();
            this.Validity.PropertyChanged += this.OnValidityChanged;
            this.ExportingRow = dataViewModel.ExportingRow;
        }

        /// <summary>
        /// Gets or sets the RSS feed Url.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 1, GroupOrderIndex = 1)]
        public DataValue<string> RssUrl
        {
            get
            {
                return this.rssUrl;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.rssUrl);
                if (this.rssUrl != null)
                {
                    this.rssUrl.PropertyChanged -= this.OnRssUrlChanged;
                }

                this.SetProperty(ref this.rssUrl, value, () => this.RssUrl);
                if (value != null)
                {
                    this.rssUrl.PropertyChanged += this.OnRssUrlChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 2, GroupOrderIndex = 1)]
        public DataValue<string> Delimiter
        {
            get
            {
                return this.delimiter;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.delimiter);
                if (this.delimiter != null)
                {
                    this.delimiter.PropertyChanged -= this.OnDelimiterChanged;
                }

                this.SetProperty(ref this.delimiter, value, () => this.Delimiter);
                if (value != null)
                {
                    this.delimiter.PropertyChanged += this.OnDelimiterChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the update interval.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 3, GroupOrderIndex = 1, FormatString = @"dd\:hh\:mm\:ss")]
        public DataValue<TimeSpan> UpdateInterval
        {
            get
            {
                return this.updateInterval;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.updateInterval);
                if (this.updateInterval != null)
                {
                    this.updateInterval.PropertyChanged -= this.OnUpdateIntervalChanged;
                }

                this.SetProperty(ref this.updateInterval, value, () => this.UpdateInterval);
                if (value != null)
                {
                    this.updateInterval.PropertyChanged += this.OnUpdateIntervalChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the validity span.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 4, GroupOrderIndex = 1, FormatString = @"dd\:hh\:mm\:ss")]
        public DataValue<TimeSpan> Validity
        {
            get
            {
                return this.validity;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.validity);
                if (this.validity != null)
                {
                    this.validity.PropertyChanged -= this.OnValidityChanged;
                }

                this.SetProperty(ref this.validity, value, () => this.Validity);
                if (value != null)
                {
                    this.validity.PropertyChanged += this.OnValidityChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the test data displayed in preview.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 8, GroupOrderIndex = 1)]
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
                    this.testData.PropertyChanged -= this.OnTestDataChanged;
                }

                this.SetProperty(ref this.testData, value, () => this.TestData);
                if (value != null)
                {
                    this.testData.PropertyChanged += this.OnTestDataChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this element is dirty.
        /// </summary>
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty || this.Align.IsDirty || this.VAlign.IsDirty || this.ScrollSpeed.IsDirty
                       || this.Validity.IsDirty || this.Delimiter.IsDirty || this.RssUrl.IsDirty
                       || this.TestData.IsDirty || this.UpdateInterval.IsDirty
                       || (this.Font != null && this.Font.IsDirty);
            }
        }

        /// <summary>
        /// Gets or sets the align.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 5, GroupOrderIndex = 1)]
        public DataValue<HorizontalAlignment> Align
        {
            get
            {
                return this.align;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.align);
                if (this.align != null)
                {
                    this.align.PropertyChanged -= this.OnAlignChanged;
                }

                this.SetProperty(ref this.align, value, () => this.Align);
                if (value != null)
                {
                    this.align.PropertyChanged += this.OnAlignChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical align.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 6, GroupOrderIndex = 1)]
        public DataValue<VerticalAlignment> VAlign
        {
            get
            {
                return this.valign;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.valign);
                if (this.valign != null)
                {
                    this.valign.PropertyChanged -= this.OnVAlignChanged;
                }

                this.SetProperty(ref this.valign, value, () => this.VAlign);
                if (value != null)
                {
                    this.valign.PropertyChanged += this.OnVAlignChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the scroll speed.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 7, GroupOrderIndex = 1)]
        public DataValue<int> ScrollSpeed
        {
            get
            {
                return this.scrollspeed;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.scrollspeed);
                if (this.scrollspeed != null)
                {
                    this.scrollspeed.PropertyChanged -= this.OnScrollSpeedChanged;
                }

                this.SetProperty(ref this.scrollspeed, value, () => this.ScrollSpeed);
                if (value != null)
                {
                    this.scrollspeed.PropertyChanged += this.OnScrollSpeedChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        public FontDataViewModel Font
        {
            get
            {
                return this.font;
            }

            set
            {
                if (this.Font != null)
                {
                    this.font.PropertyChanged -= this.OnFontChanged;
                }

                this.SetProperty(ref this.font, value, () => this.Font);
                if (value != null)
                {
                    this.font.PropertyChanged += this.OnFontChanged;
                }
            }
        }

        /// <summary>
        /// Gets or sets the font face.
        /// </summary>
        [UserVisibleProperty("Font", FieldName = "Face", OrderIndex = 0, GroupOrderIndex = 2)]
        public DataValue<string> FontFace
        {
            get
            {
                return this.Font.Face;
            }

            set
            {
                if (value != null)
                {
                    this.Font.Face = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the font height.
        /// </summary>
        [UserVisibleProperty("Font", FieldName = "Height", Filter = "TFT", OrderIndex = 1, GroupOrderIndex = 2)]
        public DataValue<int> FontHeight
        {
            get
            {
                return this.Font.Height;
            }

            set
            {
                if (value != null)
                {
                    this.Font.Height = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        [UserVisibleProperty("Font", FieldName = "Weight", Filter = "TFT", OrderIndex = 2, GroupOrderIndex = 2)]
        public DataValue<int> FontWeight
        {
            get
            {
                return this.Font.Weight;
            }

            set
            {
                if (value != null)
                {
                    this.Font.Weight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the font italic.
        /// </summary>
        [UserVisibleProperty("Font", FieldName = "Italic", Filter = "TFT", OrderIndex = 3, GroupOrderIndex = 2)]
        public DataValue<bool> FontItalic
        {
            get
            {
                return this.Font.Italic;
            }

            set
            {
                if (value != null)
                {
                    this.Font.Italic = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the font color.
        /// </summary>
        [UserVisibleProperty("Font", FieldName = "Color", OrderIndex = 4, GroupOrderIndex = 2)]
        public DataValue<string> FontColor
        {
            get
            {
                return this.Font.Color;
            }

            set
            {
                if (value != null)
                {
                    this.Font.Color = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the row to be set in the generic cell on export.
        /// </summary>
        public int ExportingRow { get; set; }

        /// <summary>
        /// Exports this element.
        /// </summary>
        /// <param name="parameters">
        /// The export Parameters.
        /// These are used to do automatic conversions if project is not compatible with the selected update group.
        /// </param>
        /// <returns>
        /// The <see cref="ElementBase"/>.
        /// </returns>
        public new ElementBase Export(object parameters = null)
        {
            var text = (TextElement)this.CreateExportObject();
            this.DoExport(text, parameters);
            return text;
        }

        /// <summary>
        /// Clears the dirty flag on all properties.
        /// </summary>
        public override void ClearDirty()
        {
            if (this.Align != null)
            {
                this.Align.ClearDirty();
            }

            if (this.VAlign != null)
            {
                this.VAlign.ClearDirty();
            }

            if (this.ScrollSpeed != null)
            {
                this.ScrollSpeed.ClearDirty();
            }

            if (this.Font != null)
            {
                this.Font.ClearDirty();
            }

            if (this.UpdateInterval.IsDirty)
            {
                this.UpdateInterval.ClearDirty();
            }

            if (this.Validity.IsDirty)
            {
                this.Validity.ClearDirty();
            }

            if (this.RssUrl.IsDirty)
            {
                this.RssUrl.ClearDirty();
            }

            if (this.Delimiter.IsDirty)
            {
                this.Delimiter.ClearDirty();
            }

            if (this.TestData.IsDirty)
            {
                this.TestData.ClearDirty();
            }

            base.ClearDirty();
        }

        /// <summary>
        /// Creates a deep clone of the current object.
        /// </summary>
        /// <returns>
        /// The deep cloned instance.
        /// </returns>
        public override object Clone()
        {
            var result = new RssTickerElementDataViewModel(this.Shell, this);
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
        public new RssTickerElementDataModel ToDataModel()
        {
            var model = (RssTickerElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(model);
            return model;
        }

        /// <summary>
        /// The equals view model.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is RssTickerElementDataViewModel)
            {
                var that = (RssTickerElementDataViewModel)obj;
                if (this != that)
                {
                    result = result && this.Align.EqualsValue(that.Align);
                    result = result && this.VAlign.EqualsValue(that.VAlign);
                    result = result && this.ScrollSpeed.EqualsValue(that.ScrollSpeed);
                    if (this.Font != null)
                    {
                        result = result && this.Font.EqualsViewModel(that.Font);
                    }
                }
            }
            else
            {
                result = false;
            }

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

            var model = dataModel as RssTickerElementDataModel;
            if (model == null)
            {
                return;
            }

            model.TestData = this.TestData.Value;
            model.Delimiter = this.Delimiter.Value;
            model.RssUrl = this.RssUrl.Value;
            model.Validity = this.Validity.Value;
            model.UpdateInterval = this.UpdateInterval.Value;
            model.ScrollSpeed = this.ScrollSpeed.Value;
            model.Align = this.Align.Value;
            model.VAlign = this.VAlign.Value;
            model.ExportingRow = this.ExportingRow;
            if (this.Font != null)
            {
                model.Font = this.Font.ToDataModel();
            }
        }

        /// <summary>
        /// Creates the export object.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object CreateExportObject()
        {
            return new TextElement();
        }

        /// <summary>
        /// Exports this instance
        /// </summary>
        /// <param name="exportModel">
        /// The export model.
        /// </param>
        /// <param name="exportParameters">
        /// The export Parameters.
        /// These are used to do automatic conversions if project is not compatible with the selected update group.
        /// </param>
        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (TextElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Align = this.Align.Value;
            model.VAlign = this.VAlign.Value;
            model.Overflow = TextOverflow.ScrollRing;
            model.ScrollSpeed = this.ScrollSpeed.Value;
            if (this.Font != null)
            {
                model.Font = this.Font.Export();
            }

            var formula = new GenericEvalDataViewModel(this.mediaShell)
                              {
                                  Column = { Value = 0 },
                                  Table = { Value = 5000 },
                                  Row = { Value = this.ExportingRow },
                                  Language = { Value = 0 }
                              };

            model.ValueProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)formula).Export());
        }

        /// <summary>
        /// Creates an empty <see cref="DynamicTextElementDataModel"/>.
        /// </summary>
        /// <returns>
        /// The the empty data model.
        /// </returns>
        protected override object CreateDataModelObject()
        {
            return new RssTickerElementDataModel();
        }

        private void OnRssUrlChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.RssUrl);
        }

        private void OnDelimiterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Delimiter);
        }

        private void OnUpdateIntervalChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.UpdateInterval);
        }

        private void OnValidityChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Validity);
        }

        private void OnAlignChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Align);
        }

        private void OnVAlignChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.VAlign);
        }

        private void OnScrollSpeedChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.ScrollSpeed);
        }

        private void OnTestDataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.TestData);
        }

        private void OnFontChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.FontFace);
            this.RaisePropertyChanged(() => this.FontHeight);
            this.RaisePropertyChanged(() => this.FontWeight);
            this.RaisePropertyChanged(() => this.FontColor);
            this.RaisePropertyChanged(() => this.FontItalic);
        }
    }
}
