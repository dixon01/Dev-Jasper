// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IconMarkupExtension.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IconMarkupExtension type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Extensions
{
    using System;
    using System.Linq;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// The icon markup extension.
    /// </summary>
    public class IconMarkupExtension : MarkupExtension
    {
        private string source;

        /// <summary>
        /// Initializes a new instance of the <see cref="IconMarkupExtension"/> class.
        /// </summary>
        public IconMarkupExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IconMarkupExtension"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        public IconMarkupExtension(string source, int size)
        {
            this.Source = source;
            this.Size = size;
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source
        {
            get
            {
                return this.source;
            }

            set
            {
                // Have to make full pack URI from short form, so System.Uri can regognize it.
                this.source = "pack://application:,,," + value;
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public int Size { get; set; }

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
            var decoder = BitmapDecoder.Create(
                new Uri(this.Source),
                BitmapCreateOptions.DelayCreation,
                BitmapCacheOption.OnDemand);

            var result = decoder.Frames.SingleOrDefault(f => Math.Abs(f.Width - this.Size) < 0.00001);
            if (result == default(BitmapFrame))
            {
                result = decoder.Frames.OrderBy(f => f.Width).First();
            }

            return result;
        }
    }
}