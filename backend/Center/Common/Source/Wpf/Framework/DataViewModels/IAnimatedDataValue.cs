// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAnimatedDataValue.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The AnimatedDataValue interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    /// <summary>
    /// The AnimatedDataValue interface.
    /// </summary>
    public interface IAnimatedDataValue : IDynamicDataValue
    {
        /// <summary>
        /// Gets or sets the animation.
        /// </summary>
        AnimationDataViewModelBase Animation { get; set; }
    }
}
