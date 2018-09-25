// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfomediaConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InfomediaConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Presentation
{
    using System;

    /// <summary>
    /// The root element of an IM2 file.
    /// </summary>
    public partial class InfomediaConfig
    {
        /// <summary>
        /// The current file version number (1.4).
        /// </summary>
        public static readonly Version CurrentVersion = new Version(1, 4);

        partial void Initialize()
        {
            this.Version = CurrentVersion;
            this.CreationDate = DateTime.Now;

            this.MasterPresentation = new MasterPresentationConfig();
            this.Cycles = new CyclesConfig();
        }
    }
}
