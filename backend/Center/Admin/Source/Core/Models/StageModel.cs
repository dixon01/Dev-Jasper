// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StageModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Windows;

    /// <summary>
    /// The data model for a stage.
    /// </summary>
    [DataContract]
    public class StageModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StageModel"/> class.
        /// </summary>
        /// <param name="name">
        /// The unique name of the stage.
        /// </param>
        public StageModel(string name)
        {
            this.Name = name;
            this.ColumnVisibilities = new Dictionary<string, Visibility>();
        }

        /// <summary>
        /// Gets or sets the name of the stage.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the hidden columns.
        /// </summary>
        [DataMember]
        public Dictionary<string, Visibility> ColumnVisibilities { get; set; }
    }
}
