// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextualReplacementDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The TextualReplacementDataViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The TextualReplacementDataViewModel.
    /// </summary>
    public class TextualReplacementDataViewModel : DataViewModelBase, ICloneable, IComparable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> filename;

        private DataValue<int> number;

        private DataValue<string> code;

        private DataValue<string> description;

        private bool isImageReplacement;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextualReplacementDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">the shell</param>
        /// <param name="commandRegistry">the command registry</param>
        /// <param name="dataModel">the dataModel</param>
        public TextualReplacementDataViewModel(
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry,
            TextualReplacementDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.CommandRegistry = commandRegistry;
            this.Number = new DataValue<int>(default(int));
            this.Number.PropertyChanged += this.NumberChanged;
            this.Code = new DataValue<string>(string.Empty);
            this.Code.PropertyChanged += this.CodeChanged;
            this.Filename = new DataValue<string>(string.Empty);
            this.Filename.PropertyChanged += this.FilenameChanged;
            this.Description = new DataValue<string>(string.Empty);
            this.Description.PropertyChanged += this.DescriptionChanged;
            if (dataModel != null)
            {
                this.Filename.Value = dataModel.Filename;
                this.Number.Value = dataModel.Number;
                this.Code.Value = dataModel.Code;
                this.Description.Value = dataModel.Description;
                this.IsImageReplacement = dataModel.IsImageReplacement;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextualReplacementDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">the shell</param>
        /// <param name="commandRegistry">the command registry</param>
        /// <param name="dataViewModel">the dataViewModel</param>
        protected TextualReplacementDataViewModel(
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry,
            TextualReplacementDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.CommandRegistry = commandRegistry;
            this.Filename = (DataValue<string>)dataViewModel.Filename.Clone();
            this.Filename.PropertyChanged += this.FilenameChanged;
            this.Number = (DataValue<int>)dataViewModel.Number.Clone();
            this.Number.PropertyChanged += this.NumberChanged;
            this.Code = (DataValue<string>)dataViewModel.Code.Clone();
            this.Code.PropertyChanged += this.CodeChanged;
            this.Description = (DataValue<string>)dataViewModel.Description.Clone();
            this.Description.PropertyChanged += this.DescriptionChanged;
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
                return base.IsDirty || this.Number.IsDirty || this.Code.IsDirty || this.Filename.IsDirty;
            }
        }

        /// <summary>
        /// Gets or sets the image resource shown in this section.
        /// </summary>
        public ResourceInfoDataViewModel Image
        {
            get
            {
                var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
                if (state.CurrentProject == null)
                {
                    return null;
                }

                var resource =
                    state.CurrentProject.Resources.FirstOrDefault(
                        model => Path.GetFileName(model.Filename) == this.Filename.Value);
                return resource;
            }

            set
            {
                if (value == null)
                {
                    this.Filename.Value = string.Empty;
                    return;
                }

                this.Filename.Value = Path.GetFileName(value.Filename);
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
        /// Gets or sets the number.
        /// Has to have a unique value between 1 and 99
        /// </summary>
        [UserVisibleProperty("Content")]
        public DataValue<int> Number
        {
            get
            {
                return this.number;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.number);

                if (this.number != null)
                {
                    this.number.PropertyChanged -= this.NumberChanged;
                }

                this.SetProperty(ref this.number, value, () => this.Number);
                if (value != null)
                {
                    this.number.PropertyChanged += this.NumberChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the code
        /// </summary>
        [UserVisibleProperty("Content")]
        public DataValue<string> Code
        {
            get
            {
                return this.code;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.code);

                if (this.code != null)
                {
                    this.code.PropertyChanged -= this.CodeChanged;
                }

                this.SetProperty(ref this.code, value, () => this.Code);
                if (value != null)
                {
                    this.code.PropertyChanged += this.CodeChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the code
        /// </summary>
        [UserVisibleProperty("Content")]
        public DataValue<string> Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.description);

                if (this.description != null)
                {
                    this.description.PropertyChanged -= this.DescriptionChanged;
                }

                this.SetProperty(ref this.description, value, () => this.Description);
                if (value != null)
                {
                    this.code.PropertyChanged += this.DescriptionChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the replacement uses an image
        /// </summary>
        public bool IsImageReplacement
        {
            get
            {
                return this.isImageReplacement;
            }

            set
            {
                this.SetProperty(ref this.isImageReplacement, value, () => this.IsImageReplacement);
            }
        }

        /// <summary>
        /// Clears the IsDirty flag. It clears the flag on the current object and all
        /// its children.
        /// </summary>
        public override void ClearDirty()
        {
            if (this.Filename != null)
            {
                this.Filename.ClearDirty();
            }

            if (this.Description != null)
            {
                this.Description.ClearDirty();
            }

            if (this.Number != null)
            {
                this.Number.ClearDirty();
            }

            if (this.Code != null)
            {
                this.Code.ClearDirty();
            }

            base.ClearDirty();
        }

        /// <summary>
        /// the clone function
        /// </summary>
        /// <returns>the cloned object</returns>
        public object Clone()
        {
            return new TextualReplacementDataViewModel(this.mediaShell, this.CommandRegistry, this)
                   {
                       ClonedFrom = this.GetHashCode(),
                       IsImageReplacement = this.IsImageReplacement,
                   };
        }

        /// <summary>
        /// Converts this data view model to an equivalent instance of <see cref="TextualReplacementDataModel"/>.
        /// </summary>
        /// <returns>A <see cref="TextualReplacementDataModel"/> equivalent to this data view model.</returns>
        public TextualReplacementDataModel ToDataModel()
        {
            var replacementDataModel = new TextualReplacementDataModel();
            this.ConvertToDataModel(replacementDataModel);
            return replacementDataModel;
        }

        /// <summary>
        /// The compare to.
        /// </summary>
        /// <param name="obj">
        /// The object
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentException("Can not compare with null.");
            }

            var other = obj as TextualReplacementDataViewModel;
            if (other == null)
            {
                throw new ArgumentException("Can not compare with different txpe.");
            }

            if (this.Number == null && other.Number == null)
            {
                return 0;
            }

            if (this.Number == null || other.Number == null)
            {
                throw new ArgumentException("Can not compare with null.");
            }

            return this.Number.Value.CompareTo(other.Number.Value);
        }

        /// <summary>
        /// Maps all properties of this class with the given data model.
        /// </summary>
        /// <param name="dataModel">
        /// The data model to map.
        /// </param>
        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (TextualReplacementDataModel)dataModel;
            if (model != null)
            {
                model.Code = this.Code.Value;
                model.Description = this.Description.Value;
                model.Filename = this.Filename.Value;
                model.Number = this.Number.Value;
                model.IsImageReplacement = this.IsImageReplacement;
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

        private void NumberChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Number);
        }

        private void CodeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Code);
        }

        private void DescriptionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Description);
        }
    }
}