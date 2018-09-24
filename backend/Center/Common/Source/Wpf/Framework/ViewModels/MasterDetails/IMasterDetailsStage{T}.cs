// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMasterDetailsStage{T}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMasterDetailsStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels.MasterDetails
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// Defines a stage with a master/detail organization with items of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    public interface IMasterDetailsStage<T> : IMasterDetailsStage
        where T : IDataViewModel
    {
        /// <summary>
        /// Gets the items list.
        /// </summary>
        ItemsListBase<T> ItemsList { get; }
    }
}