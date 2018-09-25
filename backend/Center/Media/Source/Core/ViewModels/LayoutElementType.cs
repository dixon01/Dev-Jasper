// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutElementType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The LayoutElementType.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    /// <summary>
    /// The LayoutElement types
    /// </summary>
    public enum LayoutElementType
    {
        /// <summary>
        /// The unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// the StaticText
        /// </summary>
        StaticText,

        /// <summary>
        /// the DynamicText
        /// </summary>
        DynamicText,

        /// <summary>
        /// the Image
        /// </summary>
        Image,

        /// <summary>
        /// the Video
        /// </summary>
        Video,

        /// <summary>
        /// the Frame
        /// </summary>
        Frame,

        /// <summary>
        /// the Template
        /// </summary>
        Template,

        /// <summary>
        /// The Analog Clock.
        /// </summary>
        AnalogClock,

        /// <summary>
        /// The Image List.
        /// </summary>
        ImageList,

        /// <summary>
        /// The Audio File.
        /// </summary>
        AudioFile,

        /// <summary>
        /// The static Audio TTS element.
        /// </summary>
        TextToSpeech,

        /// <summary>
        /// The dynamic Audio TTS element.
        /// </summary>
        DynamicTts,

        /// <summary>
        /// The Audio Pause element.
        /// </summary>
        AudioPause,

        /// <summary>
        /// The Rectangle element.
        /// </summary>
        Rectangle,

        /// <summary>
        /// The RSS ticker element.
        /// </summary>
        RssTicker,

        /// <summary>
        /// The Live stream element.
        /// </summary>
        LiveStream
    }
}