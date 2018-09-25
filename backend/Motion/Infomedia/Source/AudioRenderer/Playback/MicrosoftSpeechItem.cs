// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MicrosoftSpeechItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MicrosoftSpeechItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Playback
{
    using System.Speech.Synthesis;

    using NLog;

    /// <summary>
    /// Playback item that speaks a given <see cref="Prompt"/>.
    /// </summary>
    internal class MicrosoftSpeechItem : SpeechItemBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

        private Prompt prompt;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftSpeechItem"/> class.
        /// </summary>
        /// <param name="volume">
        /// The audio volume at which to speak the prompt (0..100).
        /// </param>
        public MicrosoftSpeechItem(int volume)
            : base(volume)
        {
            this.speechSynthesizer.SpeakCompleted += this.SpeechSynthesizerOnSpeakCompleted;
            this.speechSynthesizer.SpeakStarted += this.SpeechSynthesizerOnSpeakStarted;
        }

        private void SpeechSynthesizerOnSpeakStarted(object sender, SpeakStartedEventArgs speakStartedEventArgs)
        {
            Logger.Info("Microsoft TTS Speak Prompt Started  IsCompleted={0}", speakStartedEventArgs.Prompt.IsCompleted);
        }

        /// <summary>
        /// Start playing this item.
        /// </summary>
        public override void Start()
        {
            Logger.Info("Microsoft TTS SpeakAsync() {0}", this.prompt);
            this.speechSynthesizer.SpeakAsync(this.prompt);
        }

        /// <summary>
        /// Immediately stop playing this item.
        /// </summary>
        public override void Stop()
        {
            Logger.Info("Microsoft TTS SpeakAsyncCancel()");
            this.speechSynthesizer.SpeakAsyncCancel(this.prompt);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.speechSynthesizer.Dispose();
        }

        /// <summary>
        /// Configure this item.
        /// </summary>
        /// <param name="voice">
        /// The voice to be used.
        /// </param>
        /// <param name="text">
        /// The text to be spoken.
        /// </param>
        protected override void Configure(string voice, string text)
        {
            var builder = new PromptBuilder();
            builder.StartVoice(voice);
            builder.AppendText(text);
            Logger.Info("Append TTS Text Prompt[{0}]", text);
            builder.EndVoice();
            this.prompt = new Prompt(builder);
        }

        private void SpeechSynthesizerOnSpeakCompleted(object sender, SpeakCompletedEventArgs speakCompletedEventArgs)
        {
            Logger.Info("Microsoft TTS SpeakCompleted");
            this.RaiseCompleted(speakCompletedEventArgs);
        }
    }
}