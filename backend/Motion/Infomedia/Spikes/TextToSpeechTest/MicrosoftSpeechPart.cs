namespace TextToSpeechTest
{
    using System.Speech.Synthesis;

    public class MicrosoftSpeechPart : SpeechPartBase
    {
        private readonly SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

        private Prompt prompt;

        public MicrosoftSpeechPart()
        {
            this.speechSynthesizer.SpeakCompleted += this.SpeechSynthesizerOnSpeakCompleted;
        }

        public override void Configure(string text, string voice)
        {
            var builder = new PromptBuilder();
            builder.StartVoice(voice);
            builder.AppendText(text);
            builder.EndVoice();
            this.prompt = new Prompt(builder);
        }

        private void SpeechSynthesizerOnSpeakCompleted(object sender, SpeakCompletedEventArgs speakCompletedEventArgs)
        {
            this.RaiseCompleted(speakCompletedEventArgs);
        }

        public override void Start()
        {
            this.speechSynthesizer.SpeakAsync(this.prompt);
        }

        public override void Stop()
        {
            this.speechSynthesizer.SpeakAsyncCancel(this.prompt);
        }

        public override void Dispose()
        {
            this.speechSynthesizer.Dispose();
        }
    }
}