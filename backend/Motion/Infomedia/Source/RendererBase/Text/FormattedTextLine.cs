// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormattedTextLine.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FormattedTextLine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// A formatted line of text containing <see cref="TPart"/>s.
    /// Lines are always part of a <see cref="FormattedText{TPart}"/>.
    /// </summary>
    /// <typeparam name="TPart">
    /// The type of part that is in this line.
    /// </typeparam>
    public sealed class FormattedTextLine<TPart> : IDisposable
        where TPart : IPart
    {
        private readonly List<TPart> parts;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedTextLine{TPart}"/> class.
        /// </summary>
        internal FormattedTextLine()
        {
            this.parts = new List<TPart>();
            this.Parts = new ReadOnlyCollection<TPart>(this.parts);
        }

        /// <summary>
        /// Gets all parts.
        /// </summary>
        public ReadOnlyCollection<TPart> Parts { get; private set; }

        /// <summary>
        /// Gets the horizontal alignment of the line.
        /// If this value is not set, the default value from the layout element should be used.
        /// </summary>
        public HorizontalAlignment? HorizontalAlignment { get; internal set; }

        /// <summary>
        /// Duplicates this line.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// A copy of this <see cref="FormattedTextLine{TPart}"/> with all parts copied.
        /// </returns>
        public FormattedTextLine<TPart> Duplicate()
        {
            var copy = new FormattedTextLine<TPart>();
            copy.HorizontalAlignment = this.HorizontalAlignment;
            foreach (var part in this.parts)
            {
                copy.AddPart((TPart)part.Duplicate());
            }

            return copy;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var part in this.Parts)
            {
                part.Dispose();
            }
        }

        /// <summary>
        /// Adds a new part to the list of parts.
        /// </summary>
        /// <param name="part">
        /// The part to add.
        /// </param>
        internal void AddPart(TPart part)
        {
            this.parts.Add(part);
        }
    }
}