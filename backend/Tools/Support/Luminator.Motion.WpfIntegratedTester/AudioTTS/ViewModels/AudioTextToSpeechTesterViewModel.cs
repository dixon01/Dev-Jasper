namespace Luminator.Motion.WpfIntegratedTester.AudioTTS.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.IO;
    using System.Speech.Synthesis;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using System.Windows;

    using NLog;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.AudioRenderer;
    using Gorba.Motion.Infomedia.AudioRenderer;
    using Gorba.Motion.Infomedia.AudioRenderer.Playback;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.SystemManagement.Host.Path;

    using Luminator.Motion.WpfIntegratedTester.Main.ViewModels;
    using Luminator.UIFramework.Common.MVVM.ViewModelHelpers;

    public class AudioTextToSpeechTesterViewModel : BaseViewModel, IDisposable
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Task systemManagerTask;

        //private Gorba.Motion.SystemManager.ConsoleApp.Program systemManagerConsoleProgram;

        private ObservableCollection<string> speechApis;

        private string selectedTextToSpeechApi;

        private ObservableCollection<string> voices;

        private string selectedVoice;

        // ---------------------
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private FileConfigurator fileConfigurator;

        private ConfigManager<AudioRendererConfig> audioConfigManager;
        private readonly List<AudioChannelHandler> audioChannels = new List<AudioChannelHandler>();
        private AudioChannelHandler interiorAudioChannel;
        private AudioChannelHandler exteriorAudioChannel;
        private AudioChannelHandler interiorExteriorAudioChannel;
        private AudioChannelConfig interiorAudioConfig;
        private AudioChannelConfig exteriorAudioConfig;
        private AudioChannelConfig interiorExteriorAudioConfig;
        private AudioPlayer audioPlayer;
        private PlayerEngine playerEngine;
        private int systemVolume;
        private string delimeter = " ";

        private const string AppConfigFilepath = @".\App.config";
        private const string AudioRendererConfigFilepath = @".\AudioRenderer.xml";
        private const string AudioMediConfigFilepath = @".\medi.config";

        private const int TestPlaybackDuration = 8000;
        private const int TestPlaybackCooldown = 200;

        #region Single Speech Test

        private string speechText;

        private DelegateCommand defaultSpeechTextCommand;

        private DelegateCommand clearSpeechTextCommand;

        private DelegateCommand speakCommand;

        #endregion

        #region Multi Speech Test

        private ObservableCollection<string> multiSpeekTexts;

        private string selectedSpeekText;

        private DelegateCommand addSpeakMultiCommand;

        private DelegateCommand removeSpeakMultiCommand;

        private DelegateCommand defaultSpeakMultiCommand;

        private DelegateCommand clearAllSpeakMultiCommand;

        private DelegateCommand speakMultiCommand;

        #endregion

        #endregion

        #region Public Properties

        public ObservableCollection<VersionViewModel> Versions { get; set; }

        public ObservableCollection<string> SpeechApis
        {
            get { return this.speechApis; }
            set
            {
                if (this.speechApis == value) return;
                this.speechApis = value;
                this.RaisePropertyChanged(() => this.SpeechApis);
            }
        }

        public string SelectedTextToSpeechApi
        {
            get { return this.selectedTextToSpeechApi; }
            set
            {
                if (this.selectedTextToSpeechApi == value) return;
                this.selectedTextToSpeechApi = value;
                this.RaisePropertyChanged(() => this.SelectedTextToSpeechApi);

                this.SpeakCommand.RaiseCanExecuteChanged();
                this.SpeakMultiCommand.RaiseCanExecuteChanged();

                this.LoadVoices();
            }
        }

        public ObservableCollection<string> Voices
        {
            get { return this.voices; }
            set
            {
                if (this.voices == value) return;
                this.voices = value;
                this.RaisePropertyChanged(() => this.Voices);
            }
        }

        public string SelectedVoice
        {
            get { return this.selectedVoice; }
            set
            {
                if (this.selectedVoice == value) return;
                this.selectedVoice = string.IsNullOrEmpty(value) ? "" : value.Trim();
                this.RaisePropertyChanged(() => this.SelectedVoice);

                this.SpeakCommand.RaiseCanExecuteChanged();
                this.SpeakMultiCommand.RaiseCanExecuteChanged();

                this.Configure();
            }
        }

        #region Single Speech Test

        public string SpeechText
        {
            get { return this.speechText; }
            set
            {
                if (this.speechText == value) return;
                this.speechText = string.IsNullOrEmpty(value) ? "" : value.Trim();
                this.RaisePropertyChanged(() => this.SpeechText);

                this.SpeakCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand DefaultSpeechTextCommand
        {
            get
            {
                if (this.defaultSpeechTextCommand == null)
                    this.defaultSpeechTextCommand = new DelegateCommand(p => this.DefaultSpeechText(),
                        this.CanDefaultSpeechText);

                return this.defaultSpeechTextCommand;
            }
        }

        public DelegateCommand ClearSpeechTextCommand
        {
            get
            {
                if (this.clearSpeechTextCommand == null)
                    this.clearSpeechTextCommand = new DelegateCommand(p => this.ClearSpeechText(),
                        this.CanClearSpeechText);

                return this.clearSpeechTextCommand;
            }
        }

        public DelegateCommand SpeakCommand
        {
            get
            {
                if (this.speakCommand == null)
                    this.speakCommand = new DelegateCommand(p => this.Speak(), this.CanSpeak);

                return this.speakCommand;
            }
        }

        #endregion

        #region Multi Speech Test

        public ObservableCollection<string> MultiSpeekTexts
        {
            get { return this.multiSpeekTexts; }
            set
            {
                if (this.multiSpeekTexts == value) return;
                this.multiSpeekTexts = value;
                this.RaisePropertyChanged(() => this.MultiSpeekTexts);

                this.SpeakMultiCommand.RaiseCanExecuteChanged();
            }
        }

        public string SelectedSpeekText
        {
            get { return this.selectedSpeekText; }
            set
            {
                if (this.selectedSpeekText == value) return;
                this.selectedSpeekText = value;
                this.RaisePropertyChanged(() => this.SelectedSpeekText);

                this.RemoveSpeakMultiCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand AddSpeakMultiCommand
        {
            get
            {
                if (this.addSpeakMultiCommand == null)
                    this.addSpeakMultiCommand = new DelegateCommand(p => this.AddSpeakMulti(), this.CanAddSpeakMulti);

                return this.addSpeakMultiCommand;
            }
        }

        public DelegateCommand RemoveSpeakMultiCommand
        {
            get
            {
                if (this.removeSpeakMultiCommand == null)
                    this.removeSpeakMultiCommand = new DelegateCommand(p => this.RemoveSpeakMulti(),
                        this.CanRemoveSpeakMulti);

                return this.removeSpeakMultiCommand;
            }
        }

        public DelegateCommand DefaultSpeakMultiCommand
        {
            get
            {
                if (this.defaultSpeakMultiCommand == null)
                    this.defaultSpeakMultiCommand = new DelegateCommand(p => this.DefaultSpeakMulti(),
                        this.CanDefaultSpeakMulti);

                return this.defaultSpeakMultiCommand;
            }
        }

        public DelegateCommand ClearAllSpeakMultiCommand
        {
            get
            {
                if (this.clearAllSpeakMultiCommand == null)
                    this.clearAllSpeakMultiCommand = new DelegateCommand(p => this.ClearAllSpeakMulti(),
                        this.CanClearAllSpeakMulti);

                return this.clearAllSpeakMultiCommand;
            }
        }

        public DelegateCommand SpeakMultiCommand
        {
            get
            {
                if (this.speakMultiCommand == null)
                    this.speakMultiCommand = new DelegateCommand(p => this.SpeakMulti(), this.CanSpeakMulti);

                return this.speakMultiCommand;
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public AudioTextToSpeechTesterViewModel()
        {
            Versions = new ObservableCollection<VersionViewModel>();
            Voices = new ObservableCollection<string>();
            SpeechApis = new ObservableCollection<string>();
            this.speechApis.Add(Enum.GetName(typeof(TextToSpeechApi), TextToSpeechApi.Microsoft));
            this.speechApis.Add(Enum.GetName(typeof(TextToSpeechApi), TextToSpeechApi.Acapela));
            SelectedTextToSpeechApi = this.speechApis.FirstOrDefault();

            Initialize();
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {

        }

        #endregion

        #region Member Methods and Operators

        private void Initialize()
        {
            // Load Medi configuration so the MessageDispatcher gets initialized... or it will throw null ref errors
            string mediXml = File.ReadAllText(AudioMediConfigFilepath);
            MediConfig mediConfig;

            using (var reader = new StringReader(mediXml))
            {
                mediConfig = (MediConfig) new XmlSerializer(typeof(MediConfig)).Deserialize(reader);
                MessageDispatcher.Instance.Configure(
                    new ObjectConfigurator(
                        mediConfig,
                        ApplicationHelper.MachineName,
                        string.Format("AppDomain-{0}", Guid.NewGuid())));
            }

            // Load the audio configuration
            audioConfigManager = new ConfigManager<AudioRendererConfig>();
            audioConfigManager.FileName = AudioRendererConfigFilepath;
            audioConfigManager.EnableCaching = true;
            audioConfigManager.XmlSchema = AudioRendererConfig.Schema;
            var config = audioConfigManager.Config;

            logger.Trace("*** AudioRendererTest() has {0} audio channels. ***", config.AudioChannels.Count);

            // 1 = Interior
            // 2 = Exterior
            // 3 = Interior and Exterior
            foreach (var audioChannelConfig in config.AudioChannels)
            {
                // Default the config name
                foreach (var port in audioChannelConfig.SpeakerPorts)
                    port.Unit = "localhost";

                var handler = new AudioChannelHandler();
                logger.Trace("AudioRendererTest() before handler.Configure().");
                handler.Configure(audioChannelConfig, config);
                audioChannels.Add(handler);

                switch (audioChannelConfig.Id)
                {
                    case "1":
                        interiorAudioConfig = audioChannelConfig;
                        interiorAudioChannel = handler;
                        break;

                    case "2":
                        exteriorAudioConfig = audioChannelConfig;
                        exteriorAudioChannel = handler;
                        break;

                    case "3":
                        interiorExteriorAudioConfig = audioChannelConfig;
                        interiorExteriorAudioChannel = handler;
                        break;
                }
            }

            playerEngine = new PlayerEngine();

            if (config.TextToSpeech != null && config.TextToSpeech.Api == TextToSpeechApi.Acapela)
            {
                logger.Trace("AudioRendererTest() initializing AcapelaHelper. HintPath={0}",
                    config.TextToSpeech.HintPath);
                AcapelaHelper.Initialize(config.TextToSpeech.HintPath);
            }

            this.systemVolume = config.IO.VolumePort.Value;

            DefaultSpeechText();
        }

        private void LoadVoices()
        {
            SelectedVoice = string.Empty;
            this.voices.Clear();

            switch (this.SelectedTextToSpeechApi)
            {
                case "Acapela":
                {
                    if (AcapelaHelper.Engine != null)
                    {
                       foreach (var voice in AcapelaHelper.Engine.VoiceList)
                       {
                            this.voices.Add(voice);
                       }

                       SelectedVoice = voices.Contains("Sharon22k_HQ") ? "Sharon22k_HQ" : this.voices.FirstOrDefault();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Acapela library and voice files were not found in the expected locations on this system.");
                    }
                }
                    break;

                case "Microsoft":
                {
                    using (SpeechSynthesizer synth = new SpeechSynthesizer())
                    {
                        foreach (var voice in synth.GetInstalledVoices())
                        {
                            this.voices.Add(voice.VoiceInfo.Name);
                        }
                    }

                    SelectedVoice = this.voices.FirstOrDefault();
                }
                    break;

                default:
                    SelectedVoice = string.Empty;
                    break;
            }
        }

        /// <summary>
        /// Configure
        /// </summary>
        private void Configure()
        {
            // TODO configure
        }

        /// <summary>
        /// Can default the speech text
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanDefaultSpeechText(object obj)
        {
            return true;
        }

        /// <summary>
        /// Default the speech text
        /// </summary>
        private void DefaultSpeechText()
        {
            SpeechText = "This is a default text speech.  Testing 1 2 3 4.";
        }

        /// <summary>
        /// Can clear the speech text
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanClearSpeechText(object obj)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearSpeechText()
        {
            SpeechText = "";
        }

        /// <summary>
        /// Can speak single line
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanSpeak(object obj)
        {
            return
                !string.IsNullOrEmpty(this.speechText) &&
                !string.IsNullOrEmpty(this.selectedVoice);
        }

        /// <summary>
        /// Speak single line
        /// </summary>
        private void Speak()
        {
            AudioChannelHandler availableAudioChannel = null;

            if (interiorExteriorAudioChannel != null)
            {
                availableAudioChannel = interiorExteriorAudioChannel;
            }
            else if (interiorAudioChannel != null)
            {
                availableAudioChannel = interiorAudioChannel;
            }
            else
            {
                availableAudioChannel = exteriorAudioChannel;
            }

            if (availableAudioChannel == null)
            {
                MessageBox.Show(
                    "Audio channels have not been configured, please check AudioRenderer.xml configuration.");
                return;
            }

            switch (this.SelectedTextToSpeechApi)
            {
                case "Acapela":
                    audioConfigManager.Config.TextToSpeech.Api = TextToSpeechApi.Acapela;
                    break;
                case "Microsoft":
                    audioConfigManager.Config.TextToSpeech.Api = TextToSpeechApi.Microsoft;
                    break;
            }

            if (audioPlayer != null)
            {
                try
                {
                    audioPlayer.Stop();
                    audioPlayer.Dispose();
                }
                catch
                {
                    // oh well testing
                }
            }

            audioPlayer = new AudioPlayer(audioConfigManager.Config);
            switch (audioConfigManager.Config.TextToSpeech.Api)
            {
                case TextToSpeechApi.Acapela:
                    audioPlayer.AddSpeech(this.selectedVoice, this.speechText, 80);
                    break;
                case TextToSpeechApi.Microsoft:
                    audioPlayer.AddSpeech(this.selectedVoice, this.speechText, 80);
                    break;
            }
            
            playerEngine.Enqueue(1, audioPlayer, availableAudioChannel.AudioIOHandler);
            availableAudioChannel.Start(playerEngine);
        }

        /// <summary>
        /// Can add speak multi line
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanAddSpeakMulti(object obj)
        {
            return true;
        }

        /// <summary>
        /// Add speak multi line
        /// </summary>
        private void AddSpeakMulti()
        {
            this.multiSpeekTexts.Add("");
        }

        /// <summary>
        /// Can remove speak multi line
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanRemoveSpeakMulti(object obj)
        {
            return this.selectedSpeekText != null;
        }

        /// <summary>
        /// Add speak multi line
        /// </summary>
        private void RemoveSpeakMulti()
        {
            if (this.multiSpeekTexts.Contains(this.selectedSpeekText))
            {
                if (MessageBox.Show("Are you sure you want to remove the selected line?", "CONFIRM REMOVE",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.multiSpeekTexts.Remove(selectedSpeekText);
                }
            }
        }

        /// <summary>
        /// Can default speak multi line
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanDefaultSpeakMulti(object obj)
        {
            return false;
        }

        /// <summary>
        /// Default multi line
        /// </summary>
        private void DefaultSpeakMulti()
        {
            if (MessageBox.Show("Are you sure you want to set to default?", "CONFIRM DEFAULT",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.multiSpeekTexts.Clear();
                this.multiSpeekTexts.Add("This is a test of the first line");
                this.multiSpeekTexts.Add("This is a test of the second line.");
                this.multiSpeekTexts.Add("This is a test of the third line.");
                this.multiSpeekTexts.Add("This is a test of the forth line.");
                this.multiSpeekTexts.Add("This is a test of the fifth line.");
                this.multiSpeekTexts.Add("This is a test of the sixth line.");
                this.multiSpeekTexts.Add("This is a test of the seventh line.");
            }
        }

        /// <summary>
        /// Can clear speak multi line
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanClearAllSpeakMulti(object obj)
        {
            return true;
        }

        /// <summary>
        /// Clear all multi line
        /// </summary>
        private void ClearAllSpeakMulti()
        {
            if (MessageBox.Show("Are you sure you want to clear all?", "CONFIRM CLEAR",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.multiSpeekTexts.Clear();
            }
        }

        /// <summary>
        /// Can speak multi line
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanSpeakMulti(object obj)
        {
            return false;
        }

        /// <summary>
        /// Speak multi line
        /// </summary>
        private void SpeakMulti()
        {
            foreach (var line in this.multiSpeekTexts)
            {
                
            }
        }

        #endregion
    }
}
