// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddResourceParameters.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// Defines the set of parameters required to add a resource
    /// </summary>
    public class AddResourceParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddResourceParameters"/> class.
        /// </summary>
        public AddResourceParameters()
        {
            this.Completed = new TaskCompletionSource<bool>();
        }

        /// <summary>
        /// Gets or sets the resource type.
        /// </summary>
        public ResourceType Type { get; set; }

        /// <summary>
        /// Gets or sets the Media
        /// </summary>
        public IEnumerable<IFileInfo> Resources { get; set; }

        /// <summary>
        /// Gets the completed, returns so this command can be awaited.
        /// </summary>
        public TaskCompletionSource<bool> Completed { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            const string Format = "[AddResourceParameters Type: {0}, Resources: {1}]";
            return string.Format(Format, this.Type, this.Resources.Count());
        }
    }
}