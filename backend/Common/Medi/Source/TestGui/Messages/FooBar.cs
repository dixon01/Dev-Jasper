// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FooBar.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FooBar type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui.Messages
{
    using System;

    /// <summary>
    /// Simple type that contains two "built-in" properties.
    /// </summary>
    public class FooBar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FooBar"/> class.
        /// </summary>
        public FooBar()
        {
            this.Foo = Guid.NewGuid().ToString();
            this.Bar = new Random().Next(int.MaxValue);
        }

        /// <summary>
        /// Gets or sets Foo.
        /// </summary>
        public string Foo { get; set; }

        /// <summary>
        /// Gets or sets Bar.
        /// </summary>
        public int Bar { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", this.Foo, this.Bar);
        }
    }
}
