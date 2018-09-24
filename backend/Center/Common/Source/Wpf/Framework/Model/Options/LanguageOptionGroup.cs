// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageOptionGroup.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The language option model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Model.Options
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Windows.Documents;

    /// <summary>
    /// The language option group model.
    /// </summary>
    [DataContract(Name = "Group")]
    public class LanguageOptionGroup : OptionGroupBase
    {
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [DataMember]
        public string Language { get; set; }
    }
}
