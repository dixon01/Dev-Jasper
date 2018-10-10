// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model for the transformation chain editor.
    /// This editor is only used within a <see cref="TransformationPartViewModel"/>.
    /// </summary>
    public class TransformationEditorViewModel : DataErrorViewModelBase
    {
        private TransformationDataViewModelBase selectedTransformation;

        private string id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationEditorViewModel"/> class.
        /// </summary>
        public TransformationEditorViewModel()
        {
            this.Transformations = new ObservableItemCollection<TransformationDataViewModelBase>();
            this.Transformations.CollectionChanged += (sender, args) => this.MakeDirty();
            this.Transformations.ItemPropertyChanged += (sender, args) => this.MakeDirty();
        }

        /// <summary>
        /// Gets the supported types of transformations that can be added to a transformation chain.
        /// </summary>
        public Type[] SupportedTransformations
        {
            get
            {
                return TransformationDataViewModelBase.GetSupportedTransformations();
            }
        }

        /// <summary>
        /// Gets or sets the id of this transformation chain.
        /// </summary>
        public string Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.SetProperty(ref this.id, value, () => this.Id);
            }
        }

        /// <summary>
        /// Gets the list of transformations.
        /// </summary>
        public ObservableItemCollection<TransformationDataViewModelBase> Transformations { get; private set; }

        /// <summary>
        /// Gets or sets the currently selected transformation.
        /// </summary>
        public TransformationDataViewModelBase SelectedTransformation
        {
            get
            {
                return this.selectedTransformation;
            }

            set
            {
                this.SetProperty(ref this.selectedTransformation, value, () => this.SelectedTransformation);
            }
        }

        /// <summary>
        /// Gets the add transformation command.
        /// </summary>
        public ICommand AddTransformationCommand
        {
            get
            {
                return new RelayCommand<Type>(this.AddTransformation);
            }
        }

        /// <summary>
        /// Gets the remove transformation command.
        /// </summary>
        public ICommand RemoveTransformationCommand
        {
            get
            {
                return new RelayCommand(this.RemoveTransformation, this.CanRemoveTransformation);
            }
        }

        private void AddTransformation(Type info)
        {
            var index = this.Transformations.IndexOf(this.SelectedTransformation);
            index = index < 0 ? this.Transformations.Count : index + 1;
            var transformation = (TransformationConfig)Activator.CreateInstance(info);
            var viewModel = TransformationDataViewModelBase.Create(transformation);
            this.Transformations.Insert(index, viewModel);
            this.SelectedTransformation = viewModel;
        }

        private bool CanRemoveTransformation(object obj)
        {
            return this.SelectedTransformation != null;
        }

        private void RemoveTransformation()
        {
            this.Transformations.Remove(this.SelectedTransformation);
        }
    }
}