// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Language.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Language type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System.Globalization;

    /// <summary>
    /// Language information.
    /// </summary>
    public class Language
    {
        private string nativeShortName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class.
        /// </summary>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="number">
        /// The language number.
        /// </param>
        internal Language(CultureInfo culture, int number)
        {
            this.Culture = culture;
            this.Number = number;
        }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Gets the number.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Culture.TwoLetterISOLanguageName;
            }
        }

        /// <summary>
        /// Gets the native short name.
        /// </summary>
        public string NativeShortName
        {
            get
            {
                if (this.nativeShortName == null)
                {
                    this.nativeShortName = this.Culture.NativeName.Split(' ')[0];
                    this.nativeShortName = char.ToUpper(this.nativeShortName[0]) + this.nativeShortName.Substring(1);
                }

                return this.nativeShortName;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Culture);
        }
    }
}
