// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalResourceOptionGroup.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Options
{
    using System;
    using System.Runtime.Serialization;

    using Gorba.Center.Common.Wpf.Framework.Model.Options;

    /// <summary>
    /// The local resource option group.
    /// </summary>
    [DataContract(Name = "ResourceGroup")]
    public class LocalResourceOptionGroup : OptionGroupBase
    {
        /// <summary>
        /// Gets or sets the remove local resources after.
        /// </summary>
        [DataMember]
        public TimeSpan RemoveLocalResourcesAfter { get; set; }
    }
}
