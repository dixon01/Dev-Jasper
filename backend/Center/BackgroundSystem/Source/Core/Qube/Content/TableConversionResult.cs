// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableConversionResult.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TableConversionResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Qube.Content
{
    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// The result of the conversion to the table format.
    /// </summary>
    public class TableConversionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableConversionResult"/> class.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <param name="topPart">
        /// The top part.
        /// </param>
        /// <param name="bottomPart">
        /// The bottom part.
        /// </param>
        public TableConversionResult(
            ContentResource input,
            TableConversionPart output,
            TableConversionPart topPart,
            TableConversionPart bottomPart)
        {
            this.Input = input;
            this.Output = output;
            this.Bottom = bottomPart;
            this.Top = topPart;
        }

        /// <summary>
        /// Gets the input resource.
        /// </summary>
        public ContentResource Input { get; private set; }

        /// <summary>
        /// Gets the output resource.
        /// </summary>
        public TableConversionPart Output { get; private set; }

        /// <summary>
        /// Gets the bottom resource.
        /// </summary>
        public TableConversionPart Bottom { get; private set; }

        /// <summary>
        /// Gets the top resource.
        /// </summary>
        public TableConversionPart Top { get; private set; }
    }
}