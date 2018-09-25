// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CursorExtension.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The CursorExtension.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System;
    using System.Windows.Input;
    using System.Windows.Markup;

    using Gorba.Center.Media.Core.Converters;

    /// <summary>
    /// The Cursor extension.
    /// </summary>
    public class CursorExtension : MarkupExtension
    {
        /// <summary>
        /// The key.
        /// </summary>
        private string file;

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorExtension"/> class.
        /// </summary>
        /// <param name="file">
        /// The key.
        /// </param>
        public CursorExtension(string file)
        {
            this.file = file;
        }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        [ConstructorArgument("file")]
        public string File
        {
            get { return this.file; }
            set { this.file = value; }
        }

        /// <summary>
        /// Gets or sets the x position of the hotspot
        /// </summary>
        public byte X { get; set; }

        /// <summary>
        /// Gets or sets the y position of the hotspot
        /// </summary>
        public byte Y { get; set; }

        /// <summary>
        /// The provide value.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Cursor result = null;

            if (!string.IsNullOrEmpty(this.file))
            {
                var stream = EditorToolConverter.GetCursorFromIco(this.file, this.X, this.Y);
                if (stream != null)
                {
                    result = new Cursor(stream);
                }
            }

            return result;
        }
    }
}
