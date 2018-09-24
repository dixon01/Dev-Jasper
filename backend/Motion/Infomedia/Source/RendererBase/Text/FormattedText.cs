// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormattedText.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FormattedText type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Text with several parts formatted with different fonts/styles.
    /// </summary>
    /// <typeparam name="TPart">
    /// The base type of parts.
    /// </typeparam>
    public class FormattedText<TPart> : IDisposable
        where TPart : IPart
    {
        private readonly List<FormattedTextLine<TPart>> lines;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedText{TPart}"/> class.
        /// </summary>
        public FormattedText()
        {
            this.lines = new List<FormattedTextLine<TPart>>();
            this.Lines = new ReadOnlyCollection<FormattedTextLine<TPart>>(this.lines);
        }

        /// <summary>
        /// Gets all lines.
        /// </summary>
        public ReadOnlyCollection<FormattedTextLine<TPart>> Lines { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the entire text is to be shown inverted.
        /// </summary>
        public bool IsInverted { get; internal set; }

        /// <summary>
        /// Gets the vertical alignment of the entire text.
        /// If this value is not set, the default value from the layout element should be used.
        /// </summary>
        public VerticalAlignment? VerticalAlignment { get; internal set; }

        /// <summary>
        /// Gets the last line from <see cref="Lines"/>.
        /// </summary>
        /// <remarks>
        /// Use this property only if a line has already been added.
        /// </remarks>
        internal FormattedTextLine<TPart> LastLine
        {
            get
            {
                return this.lines[this.lines.Count - 1];
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            foreach (var part in this.lines)
            {
                part.Dispose();
            }
        }

        /// <summary>
        /// Add a new line to the list of lines.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        internal void AddLine(FormattedTextLine<TPart> line)
        {
            this.lines.Add(line);
        }
    }
}