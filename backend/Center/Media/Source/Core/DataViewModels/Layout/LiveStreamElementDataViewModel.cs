// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiveStreamElementDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System.ComponentModel;

    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Defines the properties of a live stream layout element.
    /// </summary>
    public class LiveStreamElementDataViewModel : VideoElementDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LiveStreamElementDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        public LiveStreamElementDataViewModel(IMediaShell mediaShell, LiveStreamElementDataModel dataModel = null)
            : base(mediaShell, dataModel)
        {
            this.FallbackImage = new DataValue<string>(string.Empty);
            this.FallbackImage.PropertyChanged += this.FallbackImageChanged;
            if (dataModel == null)
            {
                return;
            }

            this.FallbackImage.Value = dataModel.FallbackImage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveStreamElementDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        protected LiveStreamElementDataViewModel(IMediaShell mediaShell, LiveStreamElementDataViewModel dataViewModel)
            : base(mediaShell, dataViewModel)
        {
            this.FallbackImage = (DataValue<string>)dataViewModel.FallbackImage.Clone();
            this.FallbackImage.PropertyChanged += this.FallbackImageChanged;
        }

        /// <summary>
        /// The is dirty.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty || this.FallbackImage.IsDirty;
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
            var result = new LiveStreamElementDataViewModel(this.Shell, this);
            if (this.IsDirty)
            {
                result.MakeDirty();
            }

            result.ClonedFrom = this.GetHashCode();

            return result;
        }

        /// <summary>
        /// The export.
        /// </summary>
        /// <param name="exportParameters">
        /// The export Parameters.
        /// These are used to do automatic conversions if project is not compatible with the selected update group.
        /// </param>
        /// <returns>
        /// The <see cref="ElementBase"/>.
        /// </returns>
        public new ElementBase Export(object exportParameters = null)
        {
            var video = (VideoElement)this.CreateExportObject();
            this.DoExport(video, exportParameters);
            return video;
        }

        /// <summary>
        /// Converts the DataViewModel to the DataModel.
        /// </summary>
        /// <returns>
        /// The converted <see cref="LiveStreamElementDataModel"/>.
        /// </returns>
        public new LiveStreamElementDataModel ToDataModel()
        {
            var model = (LiveStreamElementDataModel)this.CreateDataModelObject();
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
            if (obj is LiveStreamElementDataViewModel)
            {
                var that = (LiveStreamElementDataViewModel)obj;
                if (this != that)
                {
                    result = result && this.VideoUri.EqualsValue(that.VideoUri);
                    result = result && this.Scaling.EqualsValue(that.Scaling);
                    result = result && this.Replay.EqualsValue(that.Replay);
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

            var model = (LiveStreamElementDataModel)dataModel;
            model.FallbackImage = this.FallbackImage.Value;
        }

        /// <summary>
        /// Creates an empty <see cref="LiveStreamElementDataModel"/>.
        /// </summary>
        /// <returns>
        /// The the empty data model.
        /// </returns>
        protected override object CreateDataModelObject()
        {
            return new LiveStreamElementDataModel();
        }

        /// <summary>
        /// The do export.
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
            base.DoExport(exportModel, exportParameters);

            var model = (VideoElement)exportModel;

            // ToDo: Set LiveStreamSpecific properties if needed.
        }

        private void FallbackImageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.FallbackImage);
        }
    }
}