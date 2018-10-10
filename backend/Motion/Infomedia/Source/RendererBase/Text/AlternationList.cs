// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlternationList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AlternationList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A list of alternations.
    /// </summary>
    /// <typeparam name="TFormattedText">
    /// The type of <see cref="FormattedText{TPart}"/> that in this list.
    /// </typeparam>
    /// <typeparam name="TPart">
    /// The base type of parts.
    /// </typeparam>
    public class AlternationList<TFormattedText, TPart> : ReadOnlyCollection<TFormattedText>
        where TFormattedText : FormattedText<TPart>
        where TPart : IPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlternationList{TFormattedText,TPart}"/> class.
        /// </summary>
        /// <param name="list">
        /// The list of items returned by this collection.
        /// </param>
        /// <param name="interval">
        /// The interval at which this text should be alternated or null for default.
        /// </param>
        public AlternationList(IList<TFormattedText> list, TimeSpan? interval)
            : base(list)
        {
            this.Interval = interval;
        }

        /// <summary>
        /// Gets the interval at which this text should be alternated.
        /// If the value is not set, the configured default value should be used.
        /// </summary>
        public TimeSpan? Interval { get; private set; }
    }
}