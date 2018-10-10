// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileNameRolloverType.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Utility.CsvFileHelper
{
    using System;

    [Flags]
    /// <summary>The file name rollover type.</summary>
    public enum FileNameRolloverType
    {
        /// <summary>The none.</summary>
        None = 0x0,

        /// <summary>The time stamp.</summary>
        TimeStampTicks = 0x1,

        /// <summary>The numerical.</summary>
        Numerical = 0x2,

        ZipArchive = 0x4
    }
}