// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityReference.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityReference type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System.Collections;

    using Gorba.Center.Admin.Core.Controllers.Entities;

    /// <summary>
    /// A reference to an entity that allows to be selected from a list.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the reference.
    /// </typeparam>
    public class EntityReference<T> : ItemSelectionViewModelBase
        where T : ReadOnlyDataViewModelBase
    {
        private T selectedEntity;

        private IReadOnlyEntityCollection<T> entities;

        /// <summary>
        /// Gets or sets the items from which the reference can be selected.
        /// </summary>
        public IReadOnlyEntityCollection<T> Entities
        {
            get
            {
                return this.entities;
            }

            set
            {
                if (this.SetProperty(ref this.entities, value, () => this.Entities))
                {
                    this.RaisePropertyChanged(() => this.Items);
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected item from the <see cref="Items"/>.
        /// </summary>
        public T SelectedEntity
        {
            get
            {
                return this.selectedEntity;
            }

            set
            {
                if (this.SetProperty(ref this.selectedEntity, value, () => this.SelectedEntity))
                {
                    this.RaisePropertyChanged(() => this.SelectedItem);
                }
            }
        }

        /// <summary>
        /// Gets the entities available for selection.
        /// </summary>
        public override ICollection Items
        {
            get
            {
                return this.Entities;
            }
        }

        /// <summary>
        /// Gets or sets the selected item from the <see cref="Items"/> collection.
        /// </summary>
        public override object SelectedItem
        {
            get
            {
                return this.SelectedEntity;
            }

            set
            {
                this.SelectedEntity = (T)value;
            }
        }
    }
}