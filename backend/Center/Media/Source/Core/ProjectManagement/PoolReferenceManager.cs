// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolReferenceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;

    /// <summary>
    /// The pool reference manager.
    /// </summary>
    public class PoolReferenceManager : ElementReferenceManagerBase<PoolSectionConfigDataViewModel>
    {
        /// <summary>
        /// Sets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void SetReferences(PoolSectionConfigDataViewModel item)
        {
            if (item.Pool == null)
            {
                return;
            }

            item.SetReference(item.Pool);
            SetReferences(item.Pool.ResourceReferences.Select(model => model.ResourceInfo), item.Pool);
        }

        /// <summary>
        /// Unsets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void UnsetReferences(PoolSectionConfigDataViewModel item)
        {
            if (item.Pool == null)
            {
                return;
            }

            UnsetReferences(item.Pool.ResourceReferences.Select(model => model.ResourceInfo), item.Pool);
            item.UnsetReference(item.Pool);
        }

        private static void SetReferences(
            IEnumerable<ReferenceTrackedDataViewModelBase> newItems,
            DataViewModelBase viewModel)
        {
            foreach (var newItem in newItems)
            {
                newItem.SetReference(viewModel);
            }
        }

        private static void UnsetReferences(
            IEnumerable<ReferenceTrackedDataViewModelBase> oldItems,
            DataViewModelBase viewModel)
        {
            foreach (var oldItem in oldItems)
            {
                oldItem.UnsetReference(viewModel);
            }
        }
    }
}