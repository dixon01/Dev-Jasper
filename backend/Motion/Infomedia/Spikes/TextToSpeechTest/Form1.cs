namespace TextToSpeechTest
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        private AudioList audioList;

        public Form1()
        {
            this.InitializeComponent();

            this.comboBoxEngine.Items.Add(new MicrosoftEngineProvider());
            this.comboBoxEngine.Items.Add(new AcapelaEngineProvider());
        }

        private IEngineProvider CurrentEngineProvider
        {
            get
            {
                return this.comboBoxEngine.SelectedItem as IEngineProvider;
            }
        }

        private void ButtonBrowseClick(object sender, System.EventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.textBoxFileName.Text = this.openFileDialog.FileName;
        }

        private void ComboBoxVoiceSelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.buttonAddText.Enabled = this.comboBoxVoice.SelectedItem != null;
        }

        private void TextBoxFileNameTextChanged(object sender, System.EventArgs e)
        {
            this.buttonAddAudioFile.Enabled = this.textBoxFileName.TextLength > 0
                                              && File.Exists(this.textBoxFileName.Text);
        }

        private void ButtonAddTextClick(object sender, System.EventArgs e)
        {
            var engineProvider = this.CurrentEngineProvider;
            if (engineProvider == null)
            {
                return;
            }

            this.listBox.Items.Add(
                new SpeakText(engineProvider, this.comboBoxVoice.Text, this.textBoxTextToSpeak.Text));
        }

        private void ButtonAddAudioFileClick(object sender, System.EventArgs e)
        {
            this.listBox.Items.Add(new PlayFile(this.textBoxFileName.Text));
        }

        private void ButtonClearClick(object sender, System.EventArgs e)
        {
            this.listBox.Items.Clear();
        }

        private void ButtonPlayClick(object sender, System.EventArgs e)
        {
            this.audioList = new AudioList();
            this.audioList.Completed += this.AudioListOnCompleted;
            foreach (var item in this.listBox.Items)
            {
                var tts = item as SpeakText;
                if (tts != null)
                {
                    var part = tts.EngineProvider.CreatePart();
                    part.Configure(tts.Text, tts.Voice);
                    this.audioList.AddSpeech(part);
                    continue;
                }

                var file = item as PlayFile;
                if (file != null)
                {
                    this.audioList.AddFile(file.FileName);
                }
            }

            this.audioList.Start();
            this.buttonPlay.Enabled = false;
            this.buttonStop.Enabled = true;
        }

        private void AudioListOnCompleted(object sender, EventArgs eventArgs)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.AudioListOnCompleted));
                return;
            }

            this.buttonPlay.Enabled = true;
            this.buttonStop.Enabled = false;
        }

        private void ButtonStopClick(object sender, EventArgs e)
        {
            this.audioList.Stop();
            this.buttonPlay.Enabled = true;
            this.buttonStop.Enabled = false;
        }

        private void ComboBoxEngineSelectedIndexChanged(object sender, EventArgs e)
        {
            this.comboBoxVoice.Items.Clear();
            this.buttonAddText.Enabled = false;

            var engineProvider = this.comboBoxEngine.SelectedItem as IEngineProvider;
            if (engineProvider == null)
            {
                return;
            }

            foreach (var voice in engineProvider.GetVoices())
            {
                this.comboBoxVoice.Items.Add(voice);
            }
        }

        private class SpeakText
        {
            public SpeakText(IEngineProvider engineProvider, string voice, string text)
            {
                this.EngineProvider = engineProvider;
                this.Voice = voice;
                this.Text = text;
            }

            public IEngineProvider EngineProvider { get; private set; }

            public string Voice { get; private set; }

            public string Text { get; private set; }

            public override string ToString()
            {
                return string.Format("{0}: \"{1}\"", this.Voice, this.Text);
            }
        }

        private class PlayFile
        {
            public PlayFile(string fileName)
            {
                this.FileName = fileName;
            }

            public string FileName { get; private set; }

            public override string ToString()
            {
                return this.FileName;
            }
        }
    }
}
