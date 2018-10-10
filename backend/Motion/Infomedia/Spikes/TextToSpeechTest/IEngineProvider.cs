namespace TextToSpeechTest
{
    using System.Collections.Generic;

    public interface IEngineProvider
    {
        string Name { get; }

        IEnumerable<string> GetVoices();

        SpeechPartBase CreatePart();
    }
}