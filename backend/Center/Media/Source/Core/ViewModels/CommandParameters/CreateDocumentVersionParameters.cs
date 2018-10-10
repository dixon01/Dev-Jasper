// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateDocumentVersionParameters.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The create document version parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    /// <summary>
    /// The create document version parameters.
    /// </summary>
    public class CreateDocumentVersionParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDocumentVersionParameters"/> class.
        /// </summary>
        /// <param name="major">
        /// The major version.
        /// </param>
        /// <param name="minor">
        /// The minor version.
        /// </param>
        /// <param name="checkInComment">
        /// The check in comment.
        /// </param>
        public CreateDocumentVersionParameters(
            int major,
            int minor,
            string checkInComment)
        {
            this.Minor = minor;
            this.Major = major;
            this.Comment = checkInComment;
        }

        /// <summary>
        /// Gets the minor.
        /// </summary>
        public int Minor { get; private set; }

        /// <summary>
        /// Gets the major.
        /// </summary>
        public int Major { get; private set; }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        public string Comment { get; private set; }
    }
}
