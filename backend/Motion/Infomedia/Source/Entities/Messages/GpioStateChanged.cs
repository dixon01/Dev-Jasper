// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="GpioStateChanged.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Common.Configuration.Infomedia.Composer;

    /// <summary>The gpio state changed.</summary>
    [Serializable]
    public class GpioStateChanged
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="GpioStateChanged"/> class.</summary>
        public GpioStateChanged()
        {
            this.GppioStates = new List<GpioState>();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether active.</summary>
        public List<GpioState> GppioStates { get; set; }

        public override string ToString()
        {
            return this.GppioStates != null
                       ? this.GppioStates.Aggregate(string.Empty, (current, g) => current + string.Format("{0}={1}, ", g.GipoName, g.Active))
                       : string.Empty;
        }

        #endregion
    }

}