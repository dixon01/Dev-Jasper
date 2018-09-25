namespace TextToSpeechTest
{
    public abstract class SpeechPartBase : AudioPart
    {
        public abstract void Configure(string text, string voice);
    }
}