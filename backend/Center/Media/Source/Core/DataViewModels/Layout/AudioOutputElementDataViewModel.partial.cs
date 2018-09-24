// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioOutputElementDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.Linq;

    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Defines the properties of an Video layout element.
    /// </summary>
    public partial class AudioOutputElementDataViewModel
    {
        private ExtendedObservableCollection<PlaybackElementDataViewModelBase> elements;

        private ExtendedObservableCollection<PlaybackElementDataViewModelBase> selectedElement;

        /// <summary>
        /// Gets the Shell.
        /// </summary>
        public IMediaShell MediaShell
        {
            get
            {
                return this.mediaShell;
            }
        }

        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        public ExtendedObservableCollection<PlaybackElementDataViewModelBase> Elements
        {
            get
            {
                return this.elements;
            }

            set
            {
                this.SetProperty(ref this.elements, value, () => this.Elements);
            }
        }

        /// <summary>
        /// Gets or sets the selected element.
        /// </summary>
        public ExtendedObservableCollection<PlaybackElementDataViewModelBase> SelectedElements
        {
            get
            {
                return this.selectedElement;
            }

            set
            {
                this.SetProperty(ref this.selectedElement, value, () => this.SelectedElements);
            }
        }

        partial void Initialize(AudioOutputElementDataModel dataModel)
        {
            this.Elements = new ExtendedObservableCollection<PlaybackElementDataViewModelBase>();
            this.SelectedElements = new ExtendedObservableCollection<PlaybackElementDataViewModelBase>();
            if (dataModel != null)
            {
                foreach (var item in dataModel.Elements)
                {
                    var typeName = item.GetType().Name.Replace("DataModel", "DataViewModel");
                    var assembly = this.GetType().Assembly;
                    var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                    if (type == null)
                    {
                        throw new Exception(
                            string.Format("Type '{0}' for data model '{1} not found", typeName, item.GetType().Name));
                    }

                    var convertedItem =
                        (PlaybackElementDataViewModelBase)Activator.CreateInstance(type, this.mediaShell, item);
                    this.Elements.Add(convertedItem);
                }
            }
        }

        partial void Initialize(AudioOutputElementDataViewModel dataViewModel)
        {
            this.Elements = new ExtendedObservableCollection<PlaybackElementDataViewModelBase>();
            this.SelectedElements = new ExtendedObservableCollection<PlaybackElementDataViewModelBase>();
            foreach (var element in dataViewModel.Elements)
            {
                var clonedItem = (PlaybackElementDataViewModelBase)element.Clone();
                this.Elements.Add(clonedItem);
            }
        }

        partial void ConvertNotGeneratedToDataModel(ref AudioOutputElementDataModel dataModel)
        {
            if (dataModel != null)
            {
                foreach (var element in this.Elements)
                {
                    var convertedItem = (PlaybackElementDataModelBase)element.ToDataModel();
                    dataModel.Elements.Add(convertedItem);
                }
            }
        }

        partial void ExportNotGeneratedValues(AudioOutputElement model, object exportParameters)
        {
            foreach (var element in this.Elements)
            {
                var convertedItem = (PlaybackElementBase)element.Export();
                model.Elements.Add(convertedItem);
            }
        }

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated)
        {
            isDirtyNotGenerated = this.Elements.IsDirty;
        }
    }
}
