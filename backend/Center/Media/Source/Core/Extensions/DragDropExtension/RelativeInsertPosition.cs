// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativeInsertPosition.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The RelativeInsertPosition.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System;

    /// <summary>
    /// The Relative Insert Position Enum
    /// </summary>
    [Flags]
    public enum RelativeInsertPosition
    {
        /// <summary>
        /// the target item before
        /// </summary>
        BeforeTargetItem = 0,

        /// <summary>
        /// the target item after
        /// </summary>
        AfterTargetItem = 1,

        /// <summary>
        /// the target item center
        /// </summary>
        TargetItemCenter = 2
    }
}