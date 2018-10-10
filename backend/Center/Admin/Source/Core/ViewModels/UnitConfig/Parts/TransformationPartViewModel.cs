// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using System.Collections.Specialized;
    using System.ComponentModel;

    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;

    /// <summary>
    /// The transformation chain part view model.
    /// </summary>
    public class TransformationPartViewModel : SingleEditorPartViewModelBase<TransformationEditorViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationPartViewModel"/> class.
        /// </summary>
        /// <param name="index">
        /// The index (from 1).
        /// </param>
        public TransformationPartViewModel(int index)
            : base(new TransformationEditorViewModel())
        {
            this.Index = index;
            this.Editor.PropertyChanged += this.EditorOnPropertyChanged;
            this.Editor.Transformations.CollectionChanged += this.TransformationsOnCollectionChanged;
        }

        /// <summary>
        /// Gets the index from 1 of this transformation in the list of all transformations.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets or sets the id of the transformation chain.
        /// </summary>
        public string ChainId
        {
            get
            {
                return this.Editor.Id;
            }

            set
            {
                this.Editor.Id = value;
            }
        }

        private void UpdateDisplayName()
        {
            this.DisplayName = string.Format("\"{0}\" ({1})", this.Editor.Id, this.Editor.Transformations.Count);
        }

        private void EditorOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Id")
            {
                this.UpdateDisplayName();
                this.RaisePropertyChanged(() => this.ChainId);
            }
        }

        private void TransformationsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateDisplayName();
        }
    }
}
