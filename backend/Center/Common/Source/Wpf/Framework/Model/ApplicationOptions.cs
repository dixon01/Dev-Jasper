// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationOptions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines global settings of the application customizable by the user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Model
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Gorba.Center.Common.Wpf.Framework.Model.Options;

    /// <summary>
    /// Defines global settings of the application customizable by the user.
    /// </summary>
    [DataContract]
    public class ApplicationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationOptions"/> class.
        /// </summary>
        public ApplicationOptions()
        {
            this.Categories = new List<OptionCategoryBase>();
        }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        [DataMember(Name = "Categories")]
        public List<OptionCategoryBase> Categories { get; set; }
    }
}
