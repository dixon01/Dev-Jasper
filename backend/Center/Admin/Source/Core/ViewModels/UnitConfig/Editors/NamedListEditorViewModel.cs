// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedListEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NamedListEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Core.Collections;

    /// <summary>
    /// The editor view model which contains a list of editable names.
    /// </summary>
    public class NamedListEditorViewModel : DataErrorViewModelBase
    {
        private NamedElement selectedElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedListEditorViewModel"/> class.
        /// </summary>
        public NamedListEditorViewModel()
        {
            this.Elements = new ObservableItemCollection<NamedElement>();
            this.Elements.ItemPropertyChanged += (s, e) => this.MakeDirty();
            this.Elements.CollectionChanged += (s, e) => this.MakeDirty();

            this.MaxElementCount = int.MaxValue;
        }

        /// <summary>
        /// Gets the list of transformation chains.
        /// </summary>
        public ObservableItemCollection<NamedElement> Elements { get; private set; }

        /// <summary>
        /// Gets or sets the maximum element count.
        /// </summary>
        public int MaxElementCount { get; set; }

        /// <summary>
        /// Gets or sets the currently selected transformation chain.
        /// </summary>
        public NamedElement SelectedElement
        {
            get
            {
                return this.selectedElement;
            }

            set
            {
                this.SetProperty(ref this.selectedElement, value, () => this.SelectedElement);
            }
        }

        /// <summary>
        /// Gets the add chain command.
        /// </summary>
        public ICommand AddElementCommand
        {
            get
            {
                return new RelayCommand(this.AddElement, this.CanAddElement);
            }
        }

        /// <summary>
        /// Gets the remove chain command.
        /// </summary>
        public ICommand RemoveElementCommand
        {
            get
            {
                return new RelayCommand(this.RemoveElement, this.CanRemoveElement);
            }
        }

        private bool CanAddElement(object obj)
        {
            return this.Elements.Count < this.MaxElementCount;
        }

        private void AddElement()
        {
            var index = this.Elements.IndexOf(this.SelectedElement);
            index = index < 0 ? this.Elements.Count : index + 1;
            var element = new NamedElement();
            this.Elements.Insert(index, element);
            this.SelectedElement = element;
        }

        private bool CanRemoveElement(object obj)
        {
            return this.SelectedElement != null;
        }

        private void RemoveElement()
        {
            this.Elements.Remove(this.SelectedElement);
        }
    }
}
