// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompatibilityChecker.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.DataViewModels.Compatibility
{
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// The CompatibilityChecker interface.
    /// </summary>
    public interface ICompatibilityChecker
    {
        /// <summary>
        /// The check.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Check(
            CompatibilityCheckParameters parameters,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages);
    }
}