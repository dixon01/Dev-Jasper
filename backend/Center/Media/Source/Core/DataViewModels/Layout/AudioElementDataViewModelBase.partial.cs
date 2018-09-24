// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioElementDataViewModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The audio element data view model base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using NLog;

    /// <summary>
    /// The audio element data view model base.
    /// </summary>
    public partial class AudioElementDataViewModelBase
    {
        private DataValue<int> listIndex;

        /// <summary>
        /// Gets the parent ViewModel.
        /// </summary>
        public AudioEditorViewModel Parent
        {
            get
            {
                return this.mediaShell.Editors[PhysicalScreenType.Audio] as AudioEditorViewModel;
            }
        }

        /// <summary>
        /// Gets or sets the element index of the <see cref="AudioOutputElementDataViewModel.Elements"/> list.
        /// </summary>
        public DataValue<int> ListIndex
        {
            get
            {
                return this.listIndex;
            }

            set
            {
                this.SetProperty(ref this.listIndex, value, () => this.ListIndex);
            }
        }

        partial void Initialize(AudioElementDataModelBase dataModel = null)
        {
            this.ListIndex = dataModel != null
                ? new DataValue<int>(dataModel.ListIndex)
                : new DataValue<int>(default(int));
            this.ListIndex.PropertyChanged += this.ListIndexChanged;
        }

        private void ListIndexChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.ListIndex);
        }

        partial void Initialize(AudioElementDataViewModelBase dataViewModel)
        {
            this.ListIndex = new DataValue<int>(dataViewModel.ListIndex.Value);
        }

        partial void ConvertNotGeneratedToDataModel(ref AudioElementDataModelBase dataModel)
        {
            if (dataModel != null)
            {
                if (this.ListIndex != null)
                {
                    dataModel.ListIndex = this.ListIndex.Value;
                }

                if (this.ElementName != null)
                {
                    dataModel.ElementName = this.ElementName.Value;
                }
            }
        }
    }
}
