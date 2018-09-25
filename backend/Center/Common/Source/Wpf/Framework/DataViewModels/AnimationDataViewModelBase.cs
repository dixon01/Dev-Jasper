// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimationDataViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The animation data view model base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    using System;

    /// <summary>
    /// The animation data view model base.
    /// </summary>
    public abstract class AnimationDataViewModelBase : DataViewModelBase, ICloneable
    {
        /// <summary>
        /// Creates a deep clone of the current object.
        /// </summary>
        /// <returns>
        /// The cloned object.
        /// </returns>
        public abstract object Clone();
    }
}
