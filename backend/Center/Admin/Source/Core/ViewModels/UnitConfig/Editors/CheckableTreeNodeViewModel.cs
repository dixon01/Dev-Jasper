// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckableTreeNodeViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CheckableTreeNodeViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System.Collections.Specialized;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core.Collections;

    /// <summary>
    /// The view model of a single node in a <see cref="CheckableTreeEditorViewModel"/>.
    /// </summary>
    public class CheckableTreeNodeViewModel : DirtyViewModelBase
    {
        private string label;

        private bool? isChecked;

        private object value;

        private bool updatingChildren;

        private bool isCheckboxVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckableTreeNodeViewModel"/> class.
        /// </summary>
        public CheckableTreeNodeViewModel()
        {
            this.Children = new ObservableItemCollection<CheckableTreeNodeViewModel>();
            this.Children.CollectionChanged += this.ChildrenOnCollectionChanged;
            this.Children.ItemPropertyChanged += this.ChildrenOnItemPropertyChanged;

            this.isChecked = false;
            this.isCheckboxVisible = true;
        }

        /// <summary>
        /// Gets or sets the label shown next to the checkbox.
        /// </summary>
        public string Label
        {
            get
            {
                return this.label;
            }

            set
            {
                this.SetProperty(ref this.label, value, () => this.Label);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this node is checked.
        /// Null means it is partially checked (some of its children are checked, some not).
        /// </summary>
        public bool? IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                if (!value.HasValue)
                {
                    value = false;
                }

                if (!this.SetProperty(ref this.isChecked, value, () => this.IsChecked))
                {
                    return;
                }

                this.MakeDirty();
                this.updatingChildren = true;
                try
                {
                    foreach (var child in this.Children)
                    {
                        child.IsChecked = value;
                    }
                }
                finally
                {
                    this.updatingChildren = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the object value of this node.
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.SetProperty(ref this.value, value, () => this.Value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the checkbox is visible.
        /// </summary>
        public bool IsCheckboxVisible
        {
            get
            {
                return this.isCheckboxVisible;
            }

            set
            {
                this.SetProperty(ref this.isCheckboxVisible, value, () => this.IsCheckboxVisible);
            }
        }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        public ObservableItemCollection<CheckableTreeNodeViewModel> Children { get; private set; }

        /// <summary>
        /// Clears the <see cref="DirtyViewModelBase.IsDirty"/> flag of this node and all its children.
        /// </summary>
        public override void ClearDirty()
        {
            base.ClearDirty();
            foreach (var child in this.Children)
            {
                child.ClearDirty();
            }
        }

        private void UpdateIsChecked()
        {
            if (this.updatingChildren)
            {
                return;
            }

            bool? newChecked = null;
            var childCount = this.Children.Count;
            if (childCount == 0)
            {
                if (this.isChecked.HasValue)
                {
                    return;
                }

                newChecked = false;
            }
            else
            {
                var checkedCount = this.Children.Count(c => c.IsChecked.HasValue && c.IsChecked.Value);
                var uncheckedCount = this.Children.Count(c => c.IsChecked.HasValue && !c.IsChecked.Value);

                if (checkedCount == childCount)
                {
                    newChecked = true;
                }
                else if (uncheckedCount == childCount)
                {
                    newChecked = false;
                }
            }

            if (this.SetProperty(ref this.isChecked, newChecked, () => this.IsChecked))
            {
                this.MakeDirty();
            }
        }

        private void ChildrenOnItemPropertyChanged(
            object sender, ItemPropertyChangedEventArgs<CheckableTreeNodeViewModel> e)
        {
            switch (e.PropertyName)
            {
                case "IsChecked":
                    this.UpdateIsChecked();
                    break;
                case "IsDirty":
                    if (e.Item.IsDirty)
                    {
                        this.MakeDirty();
                    }

                    break;
            }
        }

        private void ChildrenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateIsChecked();
            this.MakeDirty();
        }
    }
}