// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToSpeechItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextToSpeechItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    /// <summary>
    /// Playback item representing a text being spoken by TTS.
    /// </summary>
    public partial class TextToSpeechItem
    {
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("TextToSpeech: \"{0}\"", this.Value);
        }
    }
}