// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsListBase{TItem}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemsListBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels.MasterDetails
{
    using System;

    /// <summary>
    /// Defines the base class for view models used to show a list of items in a master/detail context.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public abstract class ItemsListBase<TItem> : ItemsListBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsListBase&lt;TItem&gt;"/> class.
        /// </summary>
        /// <param name="stage">The stage.</param>
        protected ItemsListBase(Lazy<IMasterDetailsStage> stage)
            : base(stage)
        {
        }
    }
}