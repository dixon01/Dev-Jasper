// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslatedText.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TranslatedText type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    /// <summary>
    /// A translated text for a certain language.
    /// </summary>
    public class TranslatedText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatedText"/> class.
        /// </summary>
        /// <param name="language">
        /// The VDV 301 language.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public TranslatedText(string language, string text)
        {
            this.Language = language;
            this.Text = text;
        }

        /// <summary>
        /// Gets the VDV 301 language.
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; private set; }
    }
}