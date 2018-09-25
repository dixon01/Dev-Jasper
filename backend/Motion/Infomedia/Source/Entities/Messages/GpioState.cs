// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="GpioState.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;

    /// <summary>The gpio state.</summary>
    [Serializable]
    public class GpioState
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="GpioState"/> class.</summary>
        /// <param name="active">The active.</param>
        /// <param name="gipoName">The gipo name.</param>
        public GpioState(string gipoName, bool active)
        {
            this.Active = active;
            this.GipoName = gipoName;
        }

        /// <summary>Initializes a new instance of the <see cref="GpioState"/> class.</summary>
        public GpioState()
        {
            this.GipoName = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether active.</summary>
        public bool Active { get; set; }

        /// <summary>Gets or sets the gipo name.</summary>
        public string GipoName { get; set; }

        #endregion
    }
}