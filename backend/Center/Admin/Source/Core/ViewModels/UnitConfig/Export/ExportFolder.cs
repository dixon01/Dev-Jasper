// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFolder.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The export folder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Common.Wpf.Core.Collections;

    /// <summary>
    /// The export folder.
    /// </summary>
    public class ExportFolder : ExportItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFolder"/> class.
        /// </summary>
        /// <param name="name">
        /// The folder name without path.
        /// </param>
        public ExportFolder(string name)
        {
            this.Name = name;
            this.Children = new ObservableItemCollection<ExportItemBase>();
            this.Children.ItemPropertyChanged += this.ChildrenOnItemPropertyChanged;
            this.Children.CollectionChanged += this.ChildrenOnCollectionChanged;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public ObservableItemCollection<ExportItemBase> Children { get; private set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public override IEnumerable<ExportItemBase> ChildItems
        {
            get
            {
                return this.Children;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item or its children have changes.
        /// </summary>
        public override bool HasChanges
        {
            get
            {
                return this.Children.Any(c => c.HasChanges);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the entity has validation errors.
        /// </summary>
        /// <returns>
        /// true if the entity currently has validation errors; otherwise, false.
        /// </returns>
        public override bool HasErrors
        {
            get
            {
                return this.Children.Any(c => c.HasErrors);
            }
        }

        /// <summary>
        /// Clears the <see cref="ExportItemBase.HasChanges"/> flag of this item and its children.
        /// </summary>
        public override void ClearHasChanges()
        {
            this.Children.ForEach(c => c.ClearHasChanges());
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for; or null or <see cref="F:System.String.Empty"/>,
        /// to retrieve entity-level errors.
        /// </param>
        public override IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && propertyName != "Children")
            {
                return null;
            }

            return
                this.Children.SelectMany(c => c.GetErrorMessages(null))
                    .Select(e => new ErrorItem(e.State, this.Name + "\\" + e.Message));
        }

        private void UpdateErrors()
        {
            this.RaiseErrorsChanged(new DataErrorsChangedEventArgs("Children"));
            this.RaisePropertyChanged(() => this.HasErrors);
        }

        private void ChildrenOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs<ExportItemBase> e)
        {
            switch (e.PropertyName)
            {
                case "HasErrors":
                    this.RaisePropertyChanged(() => this.HasErrors);
                    break;
                case "HasChanges":
                    this.RaisePropertyChanged(() => this.HasChanges);
                    break;
            }
        }

        private void ChildrenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ExportItemBase child in e.NewItems)
                {
                    child.ErrorsChanged += this.ChildOnErrorsChanged;
                    if (child.HasErrors)
                    {
                        this.UpdateErrors();
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ExportItemBase child in e.OldItems)
                {
                    child.ErrorsChanged -= this.ChildOnErrorsChanged;
                    if (child.HasErrors)
                    {
                        this.UpdateErrors();
                    }
                }
            }
        }

        private void ChildOnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            this.UpdateErrors();
        }
    }
}
