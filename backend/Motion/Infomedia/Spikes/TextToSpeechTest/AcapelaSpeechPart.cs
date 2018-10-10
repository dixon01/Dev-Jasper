namespace TextToSpeechTest
{
    using System;
    using System.Windows.Forms;

    using AcapelaGroup.BabTTSNet;

    public class AcapelaSpeechPart : SpeechPartBase
    {
        private readonly BabTTS engine = new BabTTS();

        private string textToSpeak;

        public AcapelaSpeechPart()
        {
            this.engine.OnEnd += this.EngineOnOnEnd;
        }

        public override void Configure(string text, string voice)
        {
            this.textToSpeak = text;
            this.engine.Open(voice, BabTtsOpenModes.BABTTS_DEFAULT);
        }

        public override void Start()
        {
            HandleError(
                this.engine.Speak(
                    this.textToSpeak,
                    BabTtsSpeakFlags.BABTTS_TAG_SAPI | BabTtsSpeakFlags.BABTTS_READ_TEXT | BabTtsSpeakFlags.BABTTS_ASYNC));
        }

        public override void Stop()
        {
            HandleError(this.engine.Pause());
            HandleError(this.engine.Reset());
        }

        public override void Dispose()
        {
            this.engine.Dispose();
        }

        private static void HandleError(BabTtsError error)
        {
            if (error != BabTtsError.E_BABTTS_NOERROR)
            {
                MessageBox.Show("Error: " + error, "BabTTS Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int EngineOnOnEnd()
        {
            this.RaiseCompleted(EventArgs.Empty);
            return 0;
        }
    }
}