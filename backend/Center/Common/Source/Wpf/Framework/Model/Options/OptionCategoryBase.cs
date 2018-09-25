// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionCategoryBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Model.Options
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The option category base.
    /// </summary>
    [DataContract]
    [KnownType(typeof(GeneralOptionCategory))]
    public abstract class OptionCategoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionCategoryBase"/> class.
        /// </summary>
        public OptionCategoryBase()
        {
            this.Groups = new List<OptionGroupBase>();
        }

        /// <summary>
        /// Gets or sets the option groups of this category.
        /// </summary>
        [DataMember(Name = "Groups")]
        public List<OptionGroupBase> Groups { get; set; }
    }
}
