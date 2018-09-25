// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewOne.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui.Messages
{
    using System;

    /// <summary>
    /// A new type that can be added or removed to test unknown types.
    /// </summary>
    public class NewOne
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewOne"/> class.
        /// </summary>
        public NewOne()
        {
            this.New = Guid.NewGuid().ToString();
            this.One = new Random().Next(int.MaxValue);
        }

        /// <summary>
        /// Gets or sets New.
        /// </summary>
        public string New { get; set; }

        /// <summary>
        /// Gets or sets One.
        /// </summary>
        public int One { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", this.New, this.One);
        }
    }
}
