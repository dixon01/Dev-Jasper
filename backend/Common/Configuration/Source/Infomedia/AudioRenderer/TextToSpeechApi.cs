// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToSpeechApi.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextToSpeechApi type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AudioRenderer
{
    /// <summary>
    /// The supported text-to-speech API.
    /// </summary>
    public enum TextToSpeechApi
    {
        /// <summary>
        /// The Microsoft API using <see cref="System.Speech.Synthesis"/> namespace.
        /// </summary>
        Microsoft,

        /// <summary>
        /// The Acapela API using <see cref="AcapelaGroup.BabTTSNet"/> namespace.
        /// </summary>
        Acapela
    }
}