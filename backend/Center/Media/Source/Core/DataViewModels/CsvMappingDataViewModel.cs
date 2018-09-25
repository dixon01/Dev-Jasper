// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvMappingDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CsvMappingDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;

    using NLog;

    /// <summary>
    /// Defines the data view model for a resource reference.
    /// </summary>
    public class CsvMappingDataViewModel : DataViewModelBase, ICloneable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediaShell mediaShell;

        private DataValue<string> filename;

        private DataValue<string> rawContent;

        private bool isInEditMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvMappingDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        public CsvMappingDataViewModel(
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry,
            CsvMappingDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.CommandRegistry = commandRegistry;

            this.Filename = new DataValue<string>(string.Empty);
            this.Filename.PropertyChanged += this.FilenameChanged;

            this.RawContent = new DataValue<string>(string.Empty);
            this.RawContent.PropertyChanged += this.RawContentChanged;

            if (dataModel != null)
            {
                this.Filename.Value = dataModel.Filename;
                this.RawContent.Value = dataModel.RawContent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvMappingDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        /// <param name="mappingDataViewModel">
        /// The data view model.
        /// </param>
        protected CsvMappingDataViewModel(
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry,
            CsvMappingDataViewModel mappingDataViewModel)
        {
            this.mediaShell = mediaShell;
            this.CommandRegistry = commandRegistry;
            this.Filename = (DataValue<string>)mappingDataViewModel.Filename.Clone();
            this.Filename.PropertyChanged += this.FilenameChanged;

            this.RawContent = (DataValue<string>)mappingDataViewModel.RawContent.Clone();
            this.RawContent.PropertyChanged += this.RawContentChanged;
        }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        public ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the replacement is dirty
        /// </summary>
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty || this.RawContent.IsDirty || this.Filename.IsDirty;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this resource is in edit mode
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return this.isInEditMode;
            }

            set
            {
                this.SetProperty(ref this.isInEditMode, value, () => this.IsInEditMode);
            }
        }

        /// <summary>
        /// Gets or sets the image filename
        /// </summary>
        [UserVisibleProperty("Content")]
        public DataValue<string> Filename
        {
            get
            {
                return this.filename;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.filename);

                if (this.filename != null)
                {
                    this.filename.PropertyChanged -= this.FilenameChanged;
                }

                this.SetProperty(ref this.filename, value, () => this.Filename);
                if (value != null)
                {
                    this.filename.PropertyChanged += this.FilenameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the image filename
        /// </summary>
        [UserVisibleProperty("Content")]
        public DataValue<string> RawContent
        {
            get
            {
                return this.rawContent;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.rawContent);

                if (this.rawContent != null)
                {
                    this.rawContent.PropertyChanged -= this.RawContentChanged;
                }

                this.SetProperty(ref this.rawContent, value, () => this.RawContent);
                if (value != null)
                {
                    this.rawContent.PropertyChanged += this.RawContentChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// The clear dirty.
        /// </summary>
        public override void ClearDirty()
        {
            if (this.RawContent != null)
            {
                this.RawContent.ClearDirty();
            }

            if (this.Filename != null)
            {
                this.Filename.ClearDirty();
            }

            base.ClearDirty();
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            return new CsvMappingDataViewModel(this.mediaShell, this.CommandRegistry, this)
            {
                ClonedFrom = this.GetHashCode()
            };
        }

                /// <summary>
        /// Converts this data view model to an equivalent instance of <see cref="CsvMappingDataModel"/>.
        /// </summary>
        /// <returns>A <see cref="TextualReplacementDataModel"/> equivalent to this data view model.</returns>
        public CsvMappingDataModel ToDataModel()
        {
            var csvMappingDataModel = new CsvMappingDataModel();
            this.ConvertToDataModel(csvMappingDataModel);
            return csvMappingDataModel;
        }

        /// <summary>
        /// The convert to data model.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (CsvMappingDataModel)dataModel;
            if (model != null)
            {
                model.Filename = this.Filename.Value;
                model.RawContent = this.RawContent.Value;
            }
        }

        private void FilenameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Filename);
        }

        private void RawContentChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.RawContent);
        }
    }
}