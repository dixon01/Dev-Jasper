namespace TextToSpeechTest
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Speech.Synthesis;

    public class MicrosoftEngineProvider : IEngineProvider
    {
        public string Name
        {
            get
            {
                return "Microsoft";
            }
        }

        public IEnumerable<string> GetVoices()
        {
            using (var speechSynthesizer = new SpeechSynthesizer())
            {
                return speechSynthesizer.GetInstalledVoices().Select(v => v.VoiceInfo.Name);
            }
        }

        public SpeechPartBase CreatePart()
        {
            return new MicrosoftSpeechPart();
        }
    }
}