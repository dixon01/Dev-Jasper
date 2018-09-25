// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToSpeechComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextToSpeechComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Composer that converts <see cref="TextToSpeechElement"/> to
    /// <see cref="TextToSpeechItem"/>s.
    /// </summary>
    public partial class TextToSpeechComposer
    {
        partial void Update()
        {
            this.Item.Voice = this.HandlerVoice.StringValue;
            this.Item.Value = this.HandlerValue.StringValue;
        }
    }
}