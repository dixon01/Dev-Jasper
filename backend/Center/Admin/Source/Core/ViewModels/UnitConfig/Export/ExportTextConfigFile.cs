// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportTextConfigFile.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportTextConfigFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using System.Collections;

    /// <summary>
    /// A generated (and editable) text config file to be exported.
    /// </summary>
    public class ExportTextConfigFile : ExportConfigFileBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportTextConfigFile"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="originalDocument">
        /// The original document contents.
        /// </param>
        /// <param name="contentType">
        /// The MIME type of the config file.
        /// </param>
        public ExportTextConfigFile(string fileName, string originalDocument, string contentType)
            : base(fileName, originalDocument, contentType)
        {
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for; or null or <see cref="F:System.String.Empty"/>,
        /// to retrieve entity-level errors.
        /// </param>
        public override IEnumerable GetErrors(string propertyName)
        {
            return null;
        }
    }
}
